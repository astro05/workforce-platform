using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;
using WorkforceAPI.Infrastructure.Persistence.MsSqlServer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// ── Persistence ───────────────────────────────────────────
builder.Services.AddSqlServerPersistence(
    builder.Configuration.GetConnectionString("SqlServer")!
);

// ── API ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── CORS (for React frontend) ─────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

var app = builder.Build();

// ── Migrate + Seed ────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    await DbSeeder.SeedAsync(db);
}

// ── Middleware ────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
