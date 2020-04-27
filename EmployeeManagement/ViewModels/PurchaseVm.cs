using AndyTipsterPro.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AndyTipsterPro.ViewModels
{
    public class PurchaseVm
    {
        //[Required]
        //public string FirstName { get; set; }
        //[Required]
        //public string LastName { get; set; }
        //[Required]
        //public string Email { get; set; }

        public BillingPlan Plan { get; set; }
        public Product Product { get; set; }
    }
}
