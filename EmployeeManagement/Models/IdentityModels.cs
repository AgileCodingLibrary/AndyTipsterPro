using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Identity;
using System;
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
        public bool CanSeeElitePackage { get; set; } = false;
        public DateTime ManualElitePackageAccessExpiresAt { get; set; }
        public bool CanSeeUKRacingPackage { get; set; }
        public DateTime ManualUKRacingPackageAccessExpiresAt { get; set; }
        public bool CanSeeComboPackage { get; set; }
        public DateTime ManualComboPackageAccessExpiresAt { get; set; }

        public bool BlockElitePackage { get; set; }
        public bool BlockUKRacingPackage { get; set; }
        public bool BlockComboPackage { get; set; }

    }

}