using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AndyTipsterPro.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        //remove after testing.
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Tips> Tips { get; set; }
        public DbSet<LandingPage> LandingPages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<BillingPlan> BillingPlans { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Testimonial> Testimonials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
