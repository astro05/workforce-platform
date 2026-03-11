using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public class WorkforceDbContext : DbContext
{
    public WorkforceDbContext(DbContextOptions<WorkforceDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectEmployee> ProjectEmployees => Set<ProjectEmployee>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(e =>
        {
            e.ToTable("Departments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Designation>(e =>
        {
            e.ToTable("Designations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.ToTable("Employees");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired().HasMaxLength(256);
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Salary).HasColumnType("decimal(18,2)");

            // Store Skills string[] as pipe-separated string in SQL Server
            e.Property(x => x.Skills)
             .HasMaxLength(1000)
             .HasConversion(
                 v => string.Join("|", v),
                 v => string.IsNullOrEmpty(v)
                     ? Array.Empty<string>()
                     : v.Split("|", StringSplitOptions.RemoveEmptyEntries));

            e.HasOne(x => x.Department)
             .WithMany(d => d.Employees)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Designation)
             .WithMany(d => d.Employees)
             .HasForeignKey(x => x.DesignationId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(x => x.FullName);
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.ToTable("Projects");
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        });

        modelBuilder.Entity<ProjectEmployee>(e =>
        {
            e.ToTable("ProjectEmployees");
            e.HasKey(x => new { x.ProjectId, x.EmployeeId });
            e.Property(x => x.Role).HasMaxLength(100).HasDefaultValue("Member");
            e.HasOne(x => x.Project)
             .WithMany(p => p.ProjectEmployees)
             .HasForeignKey(x => x.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Employee)
             .WithMany(emp => emp.ProjectEmployees)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("Tasks");
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.Priority).HasConversion<string>().HasMaxLength(50);
            e.HasOne(x => x.Project)
             .WithMany(p => p.Tasks)
             .HasForeignKey(x => x.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.AssignedTo)
             .WithMany(emp => emp.AssignedTasks)
             .HasForeignKey(x => x.AssignedToId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }

    // Auto-update UpdatedAt on every save
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(ct);
    }
}