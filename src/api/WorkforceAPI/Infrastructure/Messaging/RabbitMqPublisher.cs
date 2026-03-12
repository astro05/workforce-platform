using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using WorkforceAPI.Domain.Events;

namespace WorkforceAPI.Infrastructure.Messaging;

public class RabbitMqPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _initialized;

    private const string ExchangeName = "domain.events";

    public RabbitMqPublisher(
        ILogger<RabbitMqPublisher> logger,
        IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    // ── Lazy init — connect on first publish ──────────────
    private async Task EnsureConnectedAsync(
        CancellationToken ct)
    {
        if (_initialized) return;

        try
        {
            var host = _config["RabbitMQ:Host"] ?? "localhost";
            var port = int.Parse(
                _config["RabbitMQ:Port"] ?? "5672");
            var username = _config["RabbitMQ:Username"] ?? "admin";
            var password = _config["RabbitMQ:Password"] ?? "admin123";
            var vhost = _config["RabbitMQ:VirtualHost"] ?? "/";

            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = username,
                Password = password,
                VirtualHost = vhost,
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat =
                    TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout =
                    TimeSpan.FromSeconds(30),
            };

            _connection = await factory
                .CreateConnectionAsync(ct);

            _channel = await _connection
                .CreateChannelAsync(
                    cancellationToken: ct);

            // Declare exchange
            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: ct);

            _initialized = true;

            _logger.LogInformation(
                "RabbitMQ publisher connected to {Host}:{Port}",
                host, port);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "RabbitMQ not available — " +
                "events will not be published. {Error}",
                ex.Message);
        }
    }

    // ── Publish ───────────────────────────────────────────
    public async Task PublishAsync<T>(
        T domainEvent,
        CancellationToken ct = default)
        where T : BaseDomainEvent
    {
        await EnsureConnectedAsync(ct);

        if (_channel is null)
        {
            _logger.LogWarning(
                "RabbitMQ channel not available — " +
                "skipping event {EventType}",
                domainEvent.EventType);
            return;
        }

        try
        {
            var payload = new
            {
                eventId = domainEvent.EventId,
                eventType = domainEvent.EventType,
                occurredAt = domainEvent.OccurredAt,
                data = domainEvent,
            };

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(payload,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase,
                    }));

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = domainEvent.EventId
                    .ToString(),
            };

            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: domainEvent.EventType,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct);

            _logger.LogInformation(
                "Published event: {EventType}",
                domainEvent.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to publish event {EventType}",
                domainEvent.EventType);
        }
    }

    // ── Dispose ───────────────────────────────────────────
    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();
    }
}