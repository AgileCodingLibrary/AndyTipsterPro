using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
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

 
}