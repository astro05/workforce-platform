using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Application.Services;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        // DbContext
        services.AddDbContext<WorkforceDbContext>(options =>
            options.UseSqlServer(connectionString,
                sql => sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                )
            )
        );

        // Generic repositories
        services.AddScoped<IRepository<Department>, GenericRepository<Department>>();
        services.AddScoped<IRepository<Designation>, GenericRepository<Designation>>();

        // Specific repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();

        // Application Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}