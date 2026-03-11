using MongoDB.Driver;
using WorkforceAPI.API.Middleware;
using WorkforceAPI.Infrastructure.Persistence.MongoDb;
using WorkforceAPI.Infrastructure.Persistence.MsSqlServer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

// ── Persistence ───────────────────────────────────────────
builder.Services.AddSqlServerPersistence(
    builder.Configuration.GetConnectionString("SqlServer")!
);
builder.Services.AddMongoDbPersistence();

// ── API ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── CORS ──────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

var app = builder.Build();

// ── SQL Server: Migrate + Seed ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var sqlDb = scope.ServiceProvider
        .GetRequiredService<WorkforceDbContext>();
    await DbSeeder.SeedAsync(sqlDb);
}

// ── MongoDB: Seed ─────────────────────────────────────────
var mongoCtx = app.Services.GetRequiredService<MongoDbContext>();
await MongoDbSeeder.SeedAsync(mongoCtx);

// ── Middleware Pipeline ───────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();