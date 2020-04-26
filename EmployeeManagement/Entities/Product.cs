using AndyTipsterPro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndyTipsterPro.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PayPalPlanId { get; set; }         
        public int Price { get; set; }
        public string PaymentFrequency { get; set; }     
    }
}

