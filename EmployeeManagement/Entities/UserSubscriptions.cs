using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Entities
{
    public class UserSubscriptions : BaseEntity
    {

        public string PayPalAgreementId { get; set; }
        public string PayPalPlanId { get; set; }
        public string SubscriptionId { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public DateTime StartDate { get; set; } = new DateTime(2020, 01, 01);

        //Payer info
        public string PayerEmail { get; set; }
        public string PayerFirstName { get; set; }
        public string PayerLastName { get; set; }



        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}




