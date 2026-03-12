using System.Text;
using System.Text.Json;
using AuditWorker.Models;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AuditWorker.Consumers;

public class RabbitMqConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IConfiguration _config;
    private readonly AuditMongoContext _mongo;

    private IConnection? _connection;
    private IChannel? _channel;

    private readonly AsyncRetryPolicy _retryPolicy;

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        IConfiguration config,
        AuditMongoContext mongo)
    {
        _logger = logger;
        _config = config;
        _mongo = mongo;

        // Polly retry — 3 attempts with exponential backoff
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (ex, delay, attempt, _) =>
                {
                    _logger.LogWarning(
                        "Retry {Attempt} after {Delay}s — {Error}",
                        attempt, delay.TotalSeconds, ex.Message);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Wait for RabbitMQ to be ready
        await WaitForRabbitMqAsync(ct);

        // Connect
        await ConnectToRabbitMqAsync(ct);

        // Setup consumer
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (_, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await ProcessMessageAsync(message);
                });

                // Acknowledge — processed successfully
                await _channel!.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: ct);

                _logger.LogInformation(
                    "Audit log written for: {RoutingKey}",
                    args.RoutingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed after retries: {RoutingKey}",
                    args.RoutingKey);

                // Reject — don't requeue
                await _channel!.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: ct);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: "audit-worker",
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);

        _logger.LogInformation(
            "Audit Worker listening on queue: audit-worker");

        // Keep alive until cancelled
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(1000, ct);
        }
    }

    private async Task ProcessMessageAsync(string message)
    {
        var evt = JsonSerializer.Deserialize<DomainEventMessage>(
            message,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (evt is null)
        {
            _logger.LogWarning(
                "Failed to deserialize message: {Message}", message);
            return;
        }

        var auditLog = new AuditLogDocument
        {
            Id = evt.EventId.ToString(), // idempotency key
            EventType = evt.EventType,
            AggregateType = evt.AggregateType,
            AggregateId = evt.AggregateId,
            ActorName = evt.ActorName,
            Description = evt.Description,
            Before = evt.Before,
            After = evt.After,
            OccurredAt = evt.OccurredAt,
            ProcessedAt = DateTime.UtcNow
        };

        try
        {
            await _mongo.AuditLogs.InsertOneAsync(auditLog);
        }
        catch (MongoDuplicateKeyException)
        {
            // Already processed — idempotent, safe to ignore
            _logger.LogInformation(
                "Duplicate event {EventId} — skipping",
                evt.EventId);
        }
    }

    private async Task ConnectToRabbitMqAsync(CancellationToken ct)
    {
        var host = _config["RabbitMQ:Host"] ?? "localhost";
        var username = _config["RabbitMQ:Username"] ?? "admin";
        var password = _config["RabbitMQ:Password"] ?? "admin123";

        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password
        };

        // v7 async API
        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: ct);

        // Declare exchange
        await _channel.ExchangeDeclareAsync(
            exchange: "domain.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: ct);

        // Declare queue
        await _channel.QueueDeclareAsync(
            queue: "audit-worker",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        // Bind queue to exchange — listen to ALL events
        await _channel.QueueBindAsync(
            queue: "audit-worker",
            exchange: "domain.events",
            routingKey: "#",
            cancellationToken: ct);

        // Process one message at a time
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: ct);

        _logger.LogInformation(
            "Connected to RabbitMQ at {Host}", host);
    }

    private async Task WaitForRabbitMqAsync(CancellationToken ct)
    {
        var host = _config["RabbitMQ:Host"] ?? "localhost";
        var username = _config["RabbitMQ:Username"] ?? "admin";
        var password = _config["RabbitMQ:Password"] ?? "admin123";

        _logger.LogInformation(
            "Waiting for RabbitMQ at {Host}...", host);

        var retries = 0;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = host,
                    UserName = username,
                    Password = password
                };

                var testConn = await factory.CreateConnectionAsync(ct);
                await testConn.CloseAsync();
                _logger.LogInformation("RabbitMQ is ready");
                return;
            }
            catch
            {
                retries++;
                _logger.LogWarning(
                    "RabbitMQ not ready — attempt {Attempt}, retrying in 5s...",
                    retries);
                await Task.Delay(5000, ct);
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}