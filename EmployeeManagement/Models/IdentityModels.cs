using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AndyTipsterPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool SendEmails { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionDescription { get; set; }
        public string SubscriptionState { get; set; }
        public string SubscriptionEmail { get; set; }
        public string SubscriptionFirstName { get; set; }
        public string SubscriptionLastName { get; set; }
        public string SubscriptionPostalCode { get; set; }
        public string PayPalAgreementId { get; set; }

    }

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{

    //    public DbSet<Entities.Subscription> Subscriptions { get; set; }
    //    public DbSet<Entities.Questions> Questions { get; set; }
    //    public DbSet<About> Abouts { get; set; }
    //    public DbSet<Tips> Tips { get; set; }

    //    public DbSet<LandingPage> LandingPages { get; set; }
    //    //public ApplicationDbContext()
    //    //    : base("DefaultConnection")
    //    //{
    //    //}

    //}
}