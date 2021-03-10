using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using System;

namespace AndyTipsterPro.Entities
{
    public class Subscription : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string PayPalPlanId { get; set; }
        public string PayPalAgreementToken { get; set; }
        public string PayPalAgreementId { get; set; }
        public string PayPalPlanName { get; set; }
        public string PayPalPaymentEmail { get; set; }        
    }
}

