using WorkforceAPI.API.Middleware;
using WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

var builder = WebApplication.CreateBuilder(args);

// ── Persistence ───────────────────────────────────────────
builder.Services.AddSqlServerPersistence(
    builder.Configuration.GetConnectionString("SqlServer")!
);

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

// ── Migrate + Seed ────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    await DbSeeder.SeedAsync(db);
}

// ── Middleware pipeline ───────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();    //global error handler
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();