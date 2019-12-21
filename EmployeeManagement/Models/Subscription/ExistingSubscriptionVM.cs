using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AndyTipsterPro.Models.Subscription
{
    public class ExistingSubscriptionVM
    {
        //[Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        //[Required]
        public string Email { get; set; }

        public string PlanName { get; set; }
        public string PaypalAgreement { get; set; }
    }
}