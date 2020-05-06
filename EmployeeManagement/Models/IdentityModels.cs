using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AndyTipsterPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool SendEmails { get; set; }
        public List<UserSubscriptions> Subscriptions { get; set; } = new List<UserSubscriptions>();

    }

}