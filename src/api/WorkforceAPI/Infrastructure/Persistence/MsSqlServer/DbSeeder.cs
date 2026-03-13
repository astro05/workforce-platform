using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public static class DbSeeder
{
    public static async Task SeedAsync(WorkforceDbContext ctx)
    {
        // Apply only pending migrations — safe if DB already exists
        var pending = await ctx.Database
            .GetPendingMigrationsAsync();

        if (pending.Any())
            await ctx.Database.MigrateAsync();

        // Skip seeding if data already exists
        if (await ctx.Departments.AnyAsync()) return;

        // ── Departments ──────────────────────────────────────────
        var departments = new List<Department>
        {
            new() { Name = "Engineering",       Description = "Software development and architecture" },
            new() { Name = "Product",           Description = "Product management and strategy" },
            new() { Name = "Design",            Description = "UI/UX and graphic design" },
            new() { Name = "Human Resources",   Description = "People operations and recruitment" },
            new() { Name = "Finance",           Description = "Accounting and financial planning" },
            new() { Name = "Marketing",         Description = "Brand, content and growth" },
            new() { Name = "Operations",        Description = "Business operations and logistics" },
            new() { Name = "Sales",             Description = "Revenue and client relations" },
        };
        ctx.Departments.AddRange(departments);
        await ctx.SaveChangesAsync();

        // ── Designations ─────────────────────────────────────────
        var designations = new List<Designation>
        {
            new() { Name = "Junior Developer",      Level = "Junior"    },
            new() { Name = "Senior Developer",      Level = "Senior"    },
            new() { Name = "Lead Developer",        Level = "Lead"      },
            new() { Name = "Product Manager",       Level = "Mid"       },
            new() { Name = "UI/UX Designer",        Level = "Mid"       },
            new() { Name = "HR Manager",            Level = "Senior"    },
            new() { Name = "Financial Analyst",     Level = "Mid"       },
            new() { Name = "Marketing Specialist",  Level = "Junior"    },
            new() { Name = "DevOps Engineer",       Level = "Senior"    },
            new() { Name = "QA Engineer",           Level = "Mid"       },
        };
        ctx.Designations.AddRange(designations);
        await ctx.SaveChangesAsync();

        // ── Employees (50+) ───────────────────────────────────────
        var eng = departments[0];
        var prod = departments[1];
        var des = departments[2];
        var hr = departments[3];
        var fin = departments[4];
        var mkt = departments[5];
        var ops = departments[6];
        var sal = departments[7];

        var junDev = designations[0];
        var senDev = designations[1];
        var leadDev = designations[2];
        var pm = designations[3];
        var uxDes = designations[4];
        var hrMgr = designations[5];
        var finAna = designations[6];
        var mktSpc = designations[7];
        var devOps = designations[8];
        var qa = designations[9];

        var employees = new List<Employee>
        {
            // Engineering
            new() { FirstName = "James",    LastName = "Anderson",  Email = "james.anderson@workforce.com",  DepartmentId = eng.Id,  DesignationId = leadDev.Id, Salary = 9500,  JoiningDate = new DateTime(2019, 3, 1),  City = "New York",    Country = "USA",        IsActive = true,  Skills = ["C#", ".NET", "Azure", "Docker"] },
            new() { FirstName = "Sarah",    LastName = "Mitchell",  Email = "sarah.mitchell@workforce.com",  DepartmentId = eng.Id,  DesignationId = senDev.Id,  Salary = 8200,  JoiningDate = new DateTime(2020, 6, 15), City = "Austin",      Country = "USA",        IsActive = true,  Skills = ["React", "TypeScript", "Node.js"] },
            new() { FirstName = "David",    LastName = "Chen",      Email = "david.chen@workforce.com",      DepartmentId = eng.Id,  DesignationId = senDev.Id,  Salary = 8500,  JoiningDate = new DateTime(2020, 1, 10), City = "San Francisco",Country = "USA",       IsActive = true,  Skills = ["Python", "Django", "PostgreSQL"] },
            new() { FirstName = "Emily",    LastName = "Johnson",   Email = "emily.johnson@workforce.com",   DepartmentId = eng.Id,  DesignationId = junDev.Id,  Salary = 5500,  JoiningDate = new DateTime(2022, 8, 1),  City = "Seattle",     Country = "USA",        IsActive = true,  Skills = ["JavaScript", "Vue.js", "CSS"] },
            new() { FirstName = "Michael",  LastName = "Brown",     Email = "michael.brown@workforce.com",   DepartmentId = eng.Id,  DesignationId = devOps.Id,  Salary = 8800,  JoiningDate = new DateTime(2019, 11, 5), City = "Denver",      Country = "USA",        IsActive = true,  Skills = ["Docker", "Kubernetes", "CI/CD", "AWS"] },
            new() { FirstName = "Jessica",  LastName = "Taylor",    Email = "jessica.taylor@workforce.com",  DepartmentId = eng.Id,  DesignationId = junDev.Id,  Salary = 5200,  JoiningDate = new DateTime(2023, 2, 14), City = "Boston",      Country = "USA",        IsActive = true,  Skills = ["C#", "SQL", "Git"] },
            new() { FirstName = "Daniel",   LastName = "Wilson",    Email = "daniel.wilson@workforce.com",   DepartmentId = eng.Id,  DesignationId = senDev.Id,  Salary = 8000,  JoiningDate = new DateTime(2021, 4, 20), City = "Chicago",     Country = "USA",        IsActive = true,  Skills = ["Java", "Spring Boot", "MongoDB"] },
            new() { FirstName = "Ashley",   LastName = "Moore",     Email = "ashley.moore@workforce.com",    DepartmentId = eng.Id,  DesignationId = qa.Id,      Salary = 6500,  JoiningDate = new DateTime(2021, 7, 1),  City = "Portland",    Country = "USA",        IsActive = true,  Skills = ["Selenium", "Cypress", "Jest"] },
            new() { FirstName = "Ryan",     LastName = "Jackson",   Email = "ryan.jackson@workforce.com",    DepartmentId = eng.Id,  DesignationId = junDev.Id,  Salary = 5000,  JoiningDate = new DateTime(2023, 5, 10), City = "Miami",       Country = "USA",        IsActive = false, Skills = ["React", "HTML", "CSS"] },
            new() { FirstName = "Amanda",   LastName = "White",     Email = "amanda.white@workforce.com",    DepartmentId = eng.Id,  DesignationId = devOps.Id,  Salary = 9000,  JoiningDate = new DateTime(2018, 9, 3),  City = "Atlanta",     Country = "USA",        IsActive = true,  Skills = ["Terraform", "AWS", "Linux"] },

            // Product
            new() { FirstName = "Christopher",LastName="Harris",    Email = "chris.harris@workforce.com",    DepartmentId = prod.Id, DesignationId = pm.Id,      Salary = 9200,  JoiningDate = new DateTime(2019, 1, 15), City = "New York",    Country = "USA",        IsActive = true,  Skills = ["Roadmapping", "Agile", "Jira"] },
            new() { FirstName = "Megan",    LastName = "Martin",    Email = "megan.martin@workforce.com",    DepartmentId = prod.Id, DesignationId = pm.Id,      Salary = 8700,  JoiningDate = new DateTime(2020, 3, 22), City = "San Francisco",Country = "USA",       IsActive = true,  Skills = ["Scrum", "Figma", "Analytics"] },
            new() { FirstName = "Kevin",    LastName = "Thompson",  Email = "kevin.thompson@workforce.com",  DepartmentId = prod.Id, DesignationId = pm.Id,      Salary = 8000,  JoiningDate = new DateTime(2021, 6, 1),  City = "Austin",      Country = "USA",        IsActive = true,  Skills = ["Product Strategy", "OKRs"] },
            new() { FirstName = "Lauren",   LastName = "Garcia",    Email = "lauren.garcia@workforce.com",   DepartmentId = prod.Id, DesignationId = pm.Id,      Salary = 7500,  JoiningDate = new DateTime(2022, 1, 10), City = "Chicago",     Country = "USA",        IsActive = true,  Skills = ["User Research", "A/B Testing"] },

            // Design
            new() { FirstName = "Brandon",  LastName = "Martinez",  Email = "brandon.martinez@workforce.com",DepartmentId = des.Id,  DesignationId = uxDes.Id,   Salary = 7200,  JoiningDate = new DateTime(2020, 5, 18), City = "Los Angeles", Country = "USA",        IsActive = true,  Skills = ["Figma", "Adobe XD", "Prototyping"] },
            new() { FirstName = "Stephanie",LastName = "Robinson",  Email = "steph.robinson@workforce.com",  DepartmentId = des.Id,  DesignationId = uxDes.Id,   Salary = 6800,  JoiningDate = new DateTime(2021, 8, 5),  City = "Seattle",     Country = "USA",        IsActive = true,  Skills = ["Illustrator", "Photoshop", "UI Design"] },
            new() { FirstName = "Tyler",    LastName = "Clark",     Email = "tyler.clark@workforce.com",     DepartmentId = des.Id,  DesignationId = uxDes.Id,   Salary = 6500,  JoiningDate = new DateTime(2022, 3, 14), City = "Austin",      Country = "USA",        IsActive = true,  Skills = ["Sketch", "Figma", "User Testing"] },
            new() { FirstName = "Nicole",   LastName = "Rodriguez", Email = "nicole.rodriguez@workforce.com",DepartmentId = des.Id,  DesignationId = uxDes.Id,   Salary = 6200,  JoiningDate = new DateTime(2023, 1, 20), City = "Miami",       Country = "USA",        IsActive = false, Skills = ["Canva", "Motion Design"] },

            // HR
            new() { FirstName = "Jonathan", LastName = "Lewis",     Email = "jon.lewis@workforce.com",       DepartmentId = hr.Id,   DesignationId = hrMgr.Id,   Salary = 7800,  JoiningDate = new DateTime(2018, 6, 1),  City = "Dallas",      Country = "USA",        IsActive = true,  Skills = ["Recruitment", "HRIS", "Payroll"] },
            new() { FirstName = "Brittany", LastName = "Lee",       Email = "brittany.lee@workforce.com",    DepartmentId = hr.Id,   DesignationId = hrMgr.Id,   Salary = 7200,  JoiningDate = new DateTime(2019, 9, 10), City = "Phoenix",     Country = "USA",        IsActive = true,  Skills = ["Employee Relations", "Training"] },
            new() { FirstName = "Nathan",   LastName = "Walker",    Email = "nathan.walker@workforce.com",   DepartmentId = hr.Id,   DesignationId = hrMgr.Id,   Salary = 6800,  JoiningDate = new DateTime(2021, 2, 28), City = "Houston",     Country = "USA",        IsActive = true,  Skills = ["Onboarding", "Compliance"] },
            new() { FirstName = "Samantha", LastName = "Hall",      Email = "samantha.hall@workforce.com",   DepartmentId = hr.Id,   DesignationId = hrMgr.Id,   Salary = 6500,  JoiningDate = new DateTime(2022, 7, 15), City = "San Diego",   Country = "USA",        IsActive = true,  Skills = ["Talent Acquisition", "ATS"] },

            // Finance
            new() { FirstName = "Timothy",  LastName = "Allen",     Email = "tim.allen@workforce.com",       DepartmentId = fin.Id,  DesignationId = finAna.Id,  Salary = 8200,  JoiningDate = new DateTime(2018, 4, 1),  City = "New York",    Country = "USA",        IsActive = true,  Skills = ["Excel", "SAP", "Financial Modeling"] },
            new() { FirstName = "Rebecca",  LastName = "Young",     Email = "rebecca.young@workforce.com",   DepartmentId = fin.Id,  DesignationId = finAna.Id,  Salary = 7600,  JoiningDate = new DateTime(2019, 8, 20), City = "Chicago",     Country = "USA",        IsActive = true,  Skills = ["Budgeting", "Forecasting", "QuickBooks"] },
            new() { FirstName = "Patrick",  LastName = "Hernandez", Email = "patrick.h@workforce.com",       DepartmentId = fin.Id,  DesignationId = finAna.Id,  Salary = 7200,  JoiningDate = new DateTime(2020, 11, 1), City = "Boston",      Country = "USA",        IsActive = true,  Skills = ["Tax", "Audit", "GAAP"] },
            new() { FirstName = "Heather",  LastName = "King",      Email = "heather.king@workforce.com",    DepartmentId = fin.Id,  DesignationId = finAna.Id,  Salary = 6900,  JoiningDate = new DateTime(2021, 5, 17), City = "Denver",      Country = "USA",        IsActive = false, Skills = ["Payroll", "Compliance"] },

            // Marketing
            new() { FirstName = "Justin",   LastName = "Wright",    Email = "justin.wright@workforce.com",   DepartmentId = mkt.Id,  DesignationId = mktSpc.Id,  Salary = 6200,  JoiningDate = new DateTime(2020, 2, 10), City = "Los Angeles", Country = "USA",        IsActive = true,  Skills = ["SEO", "Google Ads", "Content"] },
            new() { FirstName = "Amber",    LastName = "Lopez",     Email = "amber.lopez@workforce.com",     DepartmentId = mkt.Id,  DesignationId = mktSpc.Id,  Salary = 5900,  JoiningDate = new DateTime(2021, 4, 5),  City = "Miami",       Country = "USA",        IsActive = true,  Skills = ["Social Media", "Email Marketing"] },
            new() { FirstName = "Gregory",  LastName = "Hill",      Email = "gregory.hill@workforce.com",    DepartmentId = mkt.Id,  DesignationId = mktSpc.Id,  Salary = 5700,  JoiningDate = new DateTime(2022, 6, 20), City = "Nashville",   Country = "USA",        IsActive = true,  Skills = ["Copywriting", "HubSpot"] },
            new() { FirstName = "Melissa",  LastName = "Scott",     Email = "melissa.scott@workforce.com",   DepartmentId = mkt.Id,  DesignationId = mktSpc.Id,  Salary = 5500,  JoiningDate = new DateTime(2023, 3, 1),  City = "Portland",    Country = "USA",        IsActive = true,  Skills = ["Analytics", "Brand Strategy"] },

            // Operations
            new() { FirstName = "Eric",     LastName = "Green",     Email = "eric.green@workforce.com",      DepartmentId = ops.Id,  DesignationId = pm.Id,      Salary = 7500,  JoiningDate = new DateTime(2019, 7, 14), City = "Dallas",      Country = "USA",        IsActive = true,  Skills = ["Process Improvement", "Lean"] },
            new() { FirstName = "Christina",LastName = "Adams",     Email = "christina.adams@workforce.com", DepartmentId = ops.Id,  DesignationId = pm.Id,      Salary = 7200,  JoiningDate = new DateTime(2020, 9, 30), City = "Phoenix",     Country = "USA",        IsActive = true,  Skills = ["Supply Chain", "ERP"] },
            new() { FirstName = "Steven",   LastName = "Baker",     Email = "steven.baker@workforce.com",    DepartmentId = ops.Id,  DesignationId = pm.Id,      Salary = 6800,  JoiningDate = new DateTime(2021, 11, 8), City = "Houston",     Country = "USA",        IsActive = true,  Skills = ["Project Management", "Six Sigma"] },
            new() { FirstName = "Diana",    LastName = "Gonzalez",  Email = "diana.gonzalez@workforce.com",  DepartmentId = ops.Id,  DesignationId = pm.Id,      Salary = 6500,  JoiningDate = new DateTime(2022, 4, 25), City = "San Antonio", Country = "USA",        IsActive = false, Skills = ["Vendor Management", "Logistics"] },

            // Sales
            new() { FirstName = "Brian",    LastName = "Nelson",    Email = "brian.nelson@workforce.com",    DepartmentId = sal.Id,  DesignationId = pm.Id,      Salary = 8500,  JoiningDate = new DateTime(2018, 3, 5),  City = "New York",    Country = "USA",        IsActive = true,  Skills = ["Salesforce", "Negotiation", "CRM"] },
            new() { FirstName = "Kimberly", LastName = "Carter",    Email = "kim.carter@workforce.com",      DepartmentId = sal.Id,  DesignationId = pm.Id,      Salary = 8000,  JoiningDate = new DateTime(2019, 6, 18), City = "Chicago",     Country = "USA",        IsActive = true,  Skills = ["B2B Sales", "Pipeline Management"] },
            new() { FirstName = "Mark",     LastName = "Mitchell",  Email = "mark.mitchell@workforce.com",   DepartmentId = sal.Id,  DesignationId = pm.Id,      Salary = 7500,  JoiningDate = new DateTime(2020, 10, 12),City = "Los Angeles", Country = "USA",        IsActive = true,  Skills = ["Cold Calling", "Account Management"] },
            new() { FirstName = "Laura",    LastName = "Perez",     Email = "laura.perez@workforce.com",     DepartmentId = sal.Id,  DesignationId = pm.Id,      Salary = 7000,  JoiningDate = new DateTime(2021, 1, 25), City = "Austin",      Country = "USA",        IsActive = true,  Skills = ["Lead Generation", "HubSpot"] },
            new() { FirstName = "Joseph",   LastName = "Roberts",   Email = "joseph.roberts@workforce.com",  DepartmentId = sal.Id,  DesignationId = pm.Id,      Salary = 6800,  JoiningDate = new DateTime(2022, 5, 9),  City = "Seattle",     Country = "USA",        IsActive = true,  Skills = ["SaaS Sales", "Demo Presentations"] },

            // Extra Engineering to reach 50+
            new() { FirstName = "Rachel",   LastName = "Turner",    Email = "rachel.turner@workforce.com",   DepartmentId = eng.Id,  DesignationId = senDev.Id,  Salary = 8300,  JoiningDate = new DateTime(2020, 8, 3),  City = "Boston",      Country = "USA",        IsActive = true,  Skills = ["Go", "Microservices", "gRPC"] },
            new() { FirstName = "Andrew",   LastName = "Phillips",  Email = "andrew.phillips@workforce.com", DepartmentId = eng.Id,  DesignationId = junDev.Id,  Salary = 5100,  JoiningDate = new DateTime(2023, 7, 17), City = "San Francisco",Country = "USA",       IsActive = true,  Skills = ["React", "Next.js", "Tailwind"] },
            new() { FirstName = "Jennifer", LastName = "Campbell",  Email = "jennifer.campbell@workforce.com",DepartmentId = eng.Id, DesignationId = qa.Id,      Salary = 6700,  JoiningDate = new DateTime(2021, 3, 22), City = "Denver",      Country = "USA",        IsActive = true,  Skills = ["Playwright", "API Testing", "Postman"] },
            new() { FirstName = "Scott",    LastName = "Parker",    Email = "scott.parker@workforce.com",    DepartmentId = eng.Id,  DesignationId = leadDev.Id, Salary = 9800,  JoiningDate = new DateTime(2017, 5, 1),  City = "Chicago",     Country = "USA",        IsActive = true,  Skills = ["Architecture", "C#", "SQL Server", "Redis"] },
            new() { FirstName = "Hannah",   LastName = "Evans",     Email = "hannah.evans@workforce.com",    DepartmentId = eng.Id,  DesignationId = devOps.Id,  Salary = 8600,  JoiningDate = new DateTime(2019, 12, 9), City = "Atlanta",     Country = "USA",        IsActive = true,  Skills = ["GCP", "Helm", "ArgoCD"] },
            new() { FirstName = "Joshua",   LastName = "Edwards",   Email = "joshua.edwards@workforce.com",  DepartmentId = eng.Id,  DesignationId = senDev.Id,  Salary = 8100,  JoiningDate = new DateTime(2020, 7, 27), City = "Portland",    Country = "USA",        IsActive = true,  Skills = ["Rust", "WebAssembly", "Performance"] },
            new() { FirstName = "Olivia",   LastName = "Collins",   Email = "olivia.collins@workforce.com",  DepartmentId = prod.Id, DesignationId = pm.Id,      Salary = 8900,  JoiningDate = new DateTime(2019, 4, 14), City = "New York",    Country = "USA",        IsActive = true,  Skills = ["Product Vision", "Stakeholder Mgmt"] },
            new() { FirstName = "Ethan",    LastName = "Stewart",   Email = "ethan.stewart@workforce.com",   DepartmentId = eng.Id,  DesignationId = junDev.Id,  Salary = 4900,  JoiningDate = new DateTime(2024, 1, 8),  City = "Austin",      Country = "USA",        IsActive = true,  Skills = ["Python", "FastAPI", "SQL"] },
            new() { FirstName = "Sophia",   LastName = "Sanchez",   Email = "sophia.sanchez@workforce.com",  DepartmentId = des.Id,  DesignationId = uxDes.Id,   Salary = 7000,  JoiningDate = new DateTime(2021, 10, 4), City = "Los Angeles", Country = "USA",        IsActive = true,  Skills = ["3D Design", "Blender", "Figma"] },
            new() { FirstName = "Liam",     LastName = "Morris",    Email = "liam.morris@workforce.com",     DepartmentId = eng.Id,  DesignationId = senDev.Id,  Salary = 8400,  JoiningDate = new DateTime(2020, 2, 19), City = "Seattle",     Country = "USA",        IsActive = true,  Skills = ["Kotlin", "Android", "Firebase"] },
            new() { FirstName = "Emma",     LastName = "Rogers",    Email = "emma.rogers@workforce.com",     DepartmentId = mkt.Id,  DesignationId = mktSpc.Id,  Salary = 5800,  JoiningDate = new DateTime(2022, 11, 7), City = "Nashville",   Country = "USA",        IsActive = true,  Skills = ["Influencer Marketing", "TikTok", "Instagram"] },
        };
        ctx.Employees.AddRange(employees);
        await ctx.SaveChangesAsync();

        // ── Projects ──────────────────────────────────────────────
        var projects = new List<Project>
        {
            new() { Name = "Workforce Platform",        Description = "Internal HR and workforce management system",    Status = ProjectStatus.Active,    StartDate = new DateTime(2024, 1, 1) },
            new() { Name = "Customer Portal",           Description = "Self-service portal for enterprise customers",   Status = ProjectStatus.Active,    StartDate = new DateTime(2024, 3, 1) },
            new() { Name = "Mobile App v2",             Description = "Cross-platform mobile application rewrite",      Status = ProjectStatus.OnHold,    StartDate = new DateTime(2023, 10, 1) },
            new() { Name = "Data Analytics Pipeline",   Description = "Real-time analytics and reporting platform",     Status = ProjectStatus.Active,    StartDate = new DateTime(2024, 6, 1) },
            new() { Name = "Legacy System Migration",   Description = "Migrate legacy monolith to microservices",       Status = ProjectStatus.Completed, StartDate = new DateTime(2023, 1, 1),  EndDate = new DateTime(2024, 1, 1) },
            new() { Name = "Marketing Automation",      Description = "Email and campaign automation platform",         Status = ProjectStatus.Active,    StartDate = new DateTime(2024, 4, 15) },
        };
        ctx.Projects.AddRange(projects);
        await ctx.SaveChangesAsync();

        // ── Project Members ───────────────────────────────────────
        var projectMembers = new List<ProjectEmployee>
        {
            // Workforce Platform
            new() { ProjectId = projects[0].Id, EmployeeId = employees[0].Id,  Role = "Lead"   },
            new() { ProjectId = projects[0].Id, EmployeeId = employees[1].Id,  Role = "Member" },
            new() { ProjectId = projects[0].Id, EmployeeId = employees[4].Id,  Role = "Member" },
            new() { ProjectId = projects[0].Id, EmployeeId = employees[7].Id,  Role = "Member" },
            new() { ProjectId = projects[0].Id, EmployeeId = employees[10].Id, Role = "Member" },

            // Customer Portal
            new() { ProjectId = projects[1].Id, EmployeeId = employees[2].Id,  Role = "Lead"   },
            new() { ProjectId = projects[1].Id, EmployeeId = employees[3].Id,  Role = "Member" },
            new() { ProjectId = projects[1].Id, EmployeeId = employees[14].Id, Role = "Member" },
            new() { ProjectId = projects[1].Id, EmployeeId = employees[11].Id, Role = "Member" },

            // Mobile App v2
            new() { ProjectId = projects[2].Id, EmployeeId = employees[6].Id,  Role = "Lead"   },
            new() { ProjectId = projects[2].Id, EmployeeId = employees[40].Id, Role = "Member" },
            new() { ProjectId = projects[2].Id, EmployeeId = employees[15].Id, Role = "Member" },

            // Data Analytics
            new() { ProjectId = projects[3].Id, EmployeeId = employees[43].Id, Role = "Lead"   },
            new() { ProjectId = projects[3].Id, EmployeeId = employees[9].Id,  Role = "Member" },
            new() { ProjectId = projects[3].Id, EmployeeId = employees[42].Id, Role = "Member" },

            // Marketing Automation
            new() { ProjectId = projects[5].Id, EmployeeId = employees[12].Id, Role = "Lead"   },
            new() { ProjectId = projects[5].Id, EmployeeId = employees[27].Id, Role = "Member" },
            new() { ProjectId = projects[5].Id, EmployeeId = employees[28].Id, Role = "Member" },
        };
        ctx.ProjectEmployees.AddRange(projectMembers);
        await ctx.SaveChangesAsync();

        // ── Tasks ─────────────────────────────────────────────────
        var tasks = new List<TaskItem>
        {
            // Workforce Platform tasks
            new() { Title = "Setup CI/CD Pipeline",         ProjectId = projects[0].Id, AssignedToId = employees[4].Id,  Priority = TaskPriority.High,     Status = TaskItemStatus.Done,       DueDate = new DateTime(2024, 2, 1)  },
            new() { Title = "Design Database Schema",        ProjectId = projects[0].Id, AssignedToId = employees[0].Id,  Priority = TaskPriority.Critical, Status = TaskItemStatus.Done,       DueDate = new DateTime(2024, 2, 15) },
            new() { Title = "Build Employee API",            ProjectId = projects[0].Id, AssignedToId = employees[0].Id,  Priority = TaskPriority.High,     Status = TaskItemStatus.InProgress, DueDate = new DateTime(2024, 4, 1)  },
            new() { Title = "Build React Frontend",          ProjectId = projects[0].Id, AssignedToId = employees[1].Id,  Priority = TaskPriority.High,     Status = TaskItemStatus.InProgress, DueDate = new DateTime(2024, 4, 15) },
            new() { Title = "Write Unit Tests",              ProjectId = projects[0].Id, AssignedToId = employees[7].Id,  Priority = TaskPriority.Medium,   Status = TaskItemStatus.Todo,       DueDate = new DateTime(2024, 5, 1)  },
            new() { Title = "Deploy to Production",          ProjectId = projects[0].Id, AssignedToId = employees[4].Id,  Priority = TaskPriority.Critical, Status = TaskItemStatus.Backlog,    DueDate = new DateTime(2024, 6, 1)  },

            // Customer Portal tasks
            new() { Title = "Requirements Gathering",        ProjectId = projects[1].Id, AssignedToId = employees[11].Id, Priority = TaskPriority.High,     Status = TaskItemStatus.Done,       DueDate = new DateTime(2024, 3, 15) },
            new() { Title = "UI Design Mockups",             ProjectId = projects[1].Id, AssignedToId = employees[14].Id, Priority = TaskPriority.High,     Status = TaskItemStatus.Done,       DueDate = new DateTime(2024, 4, 1)  },
            new() { Title = "API Integration",               ProjectId = projects[1].Id, AssignedToId = employees[2].Id,  Priority = TaskPriority.High,     Status = TaskItemStatus.InProgress, DueDate = new DateTime(2024, 5, 1)  },
            new() { Title = "User Acceptance Testing",       ProjectId = projects[1].Id, AssignedToId = employees[3].Id,  Priority = TaskPriority.Medium,   Status = TaskItemStatus.Todo,       DueDate = new DateTime(2024, 6, 1)  },

            // Mobile App tasks
            new() { Title = "Tech Stack Decision",           ProjectId = projects[2].Id, AssignedToId = employees[6].Id,  Priority = TaskPriority.High,     Status = TaskItemStatus.Done,       DueDate = new DateTime(2023, 11, 1) },
            new() { Title = "Auth Module",                   ProjectId = projects[2].Id, AssignedToId = employees[40].Id, Priority = TaskPriority.Critical, Status = TaskItemStatus.InReview,   DueDate = new DateTime(2024, 1, 15) },
            new() { Title = "Push Notifications",            ProjectId = projects[2].Id, AssignedToId = employees[40].Id, Priority = TaskPriority.Medium,   Status = TaskItemStatus.Backlog,    DueDate = new DateTime(2024, 3, 1)  },

            // Data Analytics tasks
            new() { Title = "Data Ingestion Pipeline",       ProjectId = projects[3].Id, AssignedToId = employees[43].Id, Priority = TaskPriority.Critical, Status = TaskItemStatus.InProgress, DueDate = new DateTime(2024, 7, 1)  },
            new() { Title = "Dashboard Design",              ProjectId = projects[3].Id, AssignedToId = employees[42].Id, Priority = TaskPriority.High,     Status = TaskItemStatus.Todo,       DueDate = new DateTime(2024, 7, 15) },
            new() { Title = "Performance Optimization",      ProjectId = projects[3].Id, AssignedToId = employees[9].Id,  Priority = TaskPriority.Medium,   Status = TaskItemStatus.Backlog,    DueDate = new DateTime(2024, 8, 1)  },

            // Marketing Automation tasks
            new() { Title = "Email Template Builder",        ProjectId = projects[5].Id, AssignedToId = employees[27].Id, Priority = TaskPriority.High,     Status = TaskItemStatus.InProgress, DueDate = new DateTime(2024, 5, 15) },
            new() { Title = "Campaign Scheduler",            ProjectId = projects[5].Id, AssignedToId = employees[12].Id, Priority = TaskPriority.High,     Status = TaskItemStatus.Todo,       DueDate = new DateTime(2024, 6, 1)  },
            new() { Title = "Analytics Dashboard",           ProjectId = projects[5].Id, AssignedToId = employees[28].Id, Priority = TaskPriority.Medium,   Status = TaskItemStatus.Backlog,    DueDate = new DateTime(2024, 7, 1)  },
        };
        ctx.Tasks.AddRange(tasks);
        await ctx.SaveChangesAsync();
    }
}