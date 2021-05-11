using challenge.Models;
using Microsoft.EntityFrameworkCore;

namespace challenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compensation>()
                .HasKey(c => new { c.Employee, c.EffectiveDate});

            modelBuilder.Entity<Employee>()
                .HasMany<Compensation>()
                .WithOne()
                .IsRequired()
                .HasForeignKey(c => c.Employee);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }
    }
}
