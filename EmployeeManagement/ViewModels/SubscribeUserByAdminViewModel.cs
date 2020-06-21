using System;
using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.ViewModels
{
    public class SubscribeUserByAdminViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool CanSeeElitePackage { get; set; }
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
