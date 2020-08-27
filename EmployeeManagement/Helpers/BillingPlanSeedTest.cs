using PayPal.v1.BillingPlans;
using PayPal.v1.Invoices;
using System.Collections.Generic;

namespace AndyTipsterPro.Helpers
{
    public static class BillingPlanSeedTest
    {

        public static List<Plan> PayPalPlans(string returnUrl, string cancelUrl) => new List<Plan>()
        {

            new Plan()
            {
                Name = "Test2 Plan with Regular payments, no startup, no trial, every third day charge",
                Description = "Test2 Plan description.",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                        Name = "Test2 Plan with Regular payments, no startup, no trial, every third day charge",
                        Type = "REGULAR",
                        Frequency = "DAY",
                        FrequencyInterval = "1",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "1.00"
                        },
                        Cycles = "0",
                        ChargeModels = new List<ChargeModel>(){
                                            new ChargeModel() {
                                               Type = "SHIPPING",
                                               Amount = new PayPal.v1.BillingPlans.Currency()
                                               {
                                                   CurrencyCode = "GBP",
                                                   Value = "0"
                                               }
                                             },
                                            new ChargeModel() {
                                               Type = "TAX",
                                               Amount = new PayPal.v1.BillingPlans.Currency()
                                               {
                                                   CurrencyCode = "GBP",
                                                   Value = "0"
                                               }
                                             }
                        },
                    }
                    //,
                    //new PaymentDefinition()
                    //{
                    //    Name = "No Trial Period",
                    //    Type = "TRIAL",
                    //    Frequency = "DAY",
                    //    FrequencyInterval = "1",
                    //    Amount = new PayPal.v1.BillingPlans.Currency()
                    //    {
                    //        CurrencyCode = "GBP",
                    //        Value = "0"
                    //    },
                    //    Cycles = "0",
                    //    ChargeModels = new List<ChargeModel>(){
                    //                        new ChargeModel() {
                    //                           Type = "SHIPPING",
                    //                           Amount = new PayPal.v1.BillingPlans.Currency()
                    //                           {
                    //                               CurrencyCode = "GBP",
                    //                               Value = "0"
                    //                           }
                    //                         },

                    //                        new ChargeModel() {
                    //                           Type = "TAX",
                    //                           Amount = new PayPal.v1.BillingPlans.Currency()
                    //                           {
                    //                               CurrencyCode = "GBP",
                    //                               Value = "0"
                    //                           }
                    //                         }
                    //    } ,
                    //}
                },

                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "1"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "0"

                }
            }

        };


    }
}

//Passing the "Setup_fee" as "£1" while creating the plan and pass the "start_date" value with the third date while creating
//    the billing agreement.So, that PayPal system will start charging the regular recurring payment from that third date onwards.

//For example, if customer/buyer sign up on 28th Aug 2020, initially he will be charged "Setup_fee" as "£1" for the 3 days and
//    you should pass the "start_date" value as 31st Aug 2020 while creating the billing agreement. 
//    So that PayPal system will start charging the regular recurring payments from the customer after 3 days(i.e 31st Aug 2020).