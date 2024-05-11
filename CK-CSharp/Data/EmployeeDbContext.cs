using CK_CSharp.configurations.model;
using Microsoft.EntityFrameworkCore;

namespace CK_CSharp.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new employeeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentModelConfiguration());
        }

        public DbSet<Models.Employee> Employees { get; set; }

        public DbSet<Models.Company> Companies { get; set; }

        public DbSet<Models.Department> Departments { get; set; }

        public DbSet<Models.Category> Categories { get; set; }

        public DbSet<Models.Schedule> schedules { get; set; }

        public DbSet<Models.Announcement> announcements { get; set; }


    }
}
