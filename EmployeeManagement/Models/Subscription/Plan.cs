using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AndyTipsterPro.Models.Subscription
{
    public class Plan
    {
        public string PayPalPlanId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }


        public static List<Plan> Plans => new List<Plan>()
        {
            new Plan()
            {
                Name = "AndyTipster Monthly Package",
                Price = 1300,
                PayPalPlanId = "P-5WM34361FF382452MBTIZJKI", // Created in the BillingPlansController.
                Description = "Membership group that's amassed Thousands of ££ Profit Since Feb 2013",
            },
            new Plan()
            {
                Name = "AndyTipster Three Months Package",
                Price = 3500,
                PayPalPlanId = "P-4J86689303992920LBTIZNEA", // Created in the BillingPlansController.
                Description = "Membership group that's amassed Thousands of ££ Profit Since Feb 2013",
            },
              new Plan()
            {
                Name = "Irish Horse Racing - Monthly Subscription",
                Price = 1000,
                PayPalPlanId = "P-4HF95636W6960994BBTIZQ3Q", // Created in the BillingPlansController.
                Description = "Proven Success on facebook With Years of Gains at level Stakes",
            },
                  new Plan()
            {
                Name = "Irish Horse Racing - Three Months Subscription",
                Price = 2750,
                PayPalPlanId = "P-18W59659DP119424LBTIZUUY", // Created in the BillingPlansController.
                Description = "Proven Success on facebook With Years of Gains at level Stakes",
            },
                 new Plan()
            {
                Name = "Ultimate pack - Monthly Subscription",
                Price = 1700,
                PayPalPlanId = "P-1DW5554726310603GBTIZYHY", // Created in the BillingPlansController.
                Description = "The combination of all sports- Horse Racing from the UK and IRE aswell as other sports",
            },
                      new Plan()
            {
                Name = "Ultimate pack - Three Months Subscription",
                Price = 4500,
                PayPalPlanId = "P-9PL12317MH697663WBTIZ36I", // Created in the BillingPlansController.
                Description = "The combination of all sports- Horse Racing from the UK and IRE aswell as other sports",
            }
        };
    }


}