using System;

namespace AndyTipsterPro.ViewModels
{
    public class Subscriber
    {

        //user details

        public string ApplicationUserFirstName { get; set; }
        public string ApplicationUserLastName { get; set; }
        public string ApplicationUserEmail { get; set; }

        //subscription details
        public string PayerFirstName { get; set; }
        public string PayerLastName { get; set; }
        public string PayerEmail { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string PayPalPlanId { get; set; }      
        public string PayPalAgreementId { get; set; }
        public string PayPalPlanDescription { get; set; }
        
    }
}
