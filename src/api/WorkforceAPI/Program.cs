using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

var builder = WebApplication.CreateBuilder(args);

// DbContext - SQL Server
builder.Services.AddDbContext<WorkforceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── Auto migrate + seed on startup ───────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();