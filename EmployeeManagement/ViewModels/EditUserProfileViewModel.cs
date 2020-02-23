using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.ViewModels
{
    public class EditUserProfileViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        public bool SendEmails { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        //subscription properties.
        public string SubscriptionId { get; set; }
        public string SubscriptionDescription { get; set; }
        public string SubscriptionState { get; set; }
        public string SubscriptionEmail { get; set; }
        public string SubscriptionFirstName { get; set; }
        public string SubscriptionLastName { get; set; }
        public string SubscriptionPostalCode { get; set; }
        public string PayPalAgreementId { get; set; }
    }


}