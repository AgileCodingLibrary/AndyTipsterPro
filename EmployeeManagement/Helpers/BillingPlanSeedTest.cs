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
                Name = "Elite Package - Monthly (092020)",
                Description = "Elite Package - Monthly (092020)",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                        Name = "Regular Payments",
                        Type = "REGULAR",
                        Frequency = "MONTH",
                        FrequencyInterval = "1",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "28.00"
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
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "28.00"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "1"

                }
            },
           new Plan()
            {
                 Name = "Elite Package - 3 Months (092020)",
                Description = "Elite Package - 3 Months (092020)",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                         Name = "Regular Payments",
                        Type = "REGULAR",
                        Frequency = "MONTH",
                        FrequencyInterval = "3",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "70.00"
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
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "70.00"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "1"

                }
            },
           new Plan()
            {
               Name = "Combination Package UK/IRE - Monthly (092020)",
                Description = "Combination Package UK/IRE - Monthly (092020)",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                         Name = "Regular Payments",
                        Type = "REGULAR",
                        Frequency = "MONTH",
                        FrequencyInterval = "1",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "24.00"
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
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "24.00"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "1"

                }
            },
           new Plan()
            {
                Name = "Combination Package UK/IRE -  3 Months (092020)",
                Description = "Combination Package UK/IRE -  3 Months (092020)",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                        Name = "Regular Payments",
                        Type = "REGULAR",
                        Frequency = "MONTH",
                        FrequencyInterval = "3",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "60.00"
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
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                             Value = "60.00"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "1"

                }
            },
           new Plan()
            {
                 Name = "UK Racing Only Package - Monthly (092020)",
                Description = "UK Racing Only Package - Monthly (092020)",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                        Name = "Regular Payments",
                        Type = "REGULAR",
                        Frequency = "MONTH",
                        FrequencyInterval = "1",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "19.00"
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
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                             Value = "19.00"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "1"

                }
            },
           new Plan()
            {
                Name = "UK Racing Only Package - 3 Months (092020)",
                Description = "UK Racing Only Package - 3 Months (092020)",
                Type = "infinite",
                State = "ACTIVE",
                PaymentDefinitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                        Name = "Regular Payments",
                        Type = "REGULAR",
                        Frequency = "MONTH",
                        FrequencyInterval = "3",
                        Amount = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "50.00"
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
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    SetupFee = new PayPal.v1.BillingPlans.Currency()
                        {
                            CurrencyCode = "GBP",
                             Value = "50.00"
                        },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    AutoBillAmount = "NO",
                    InitialFailAmountAction = "CANCEL",
                    MaxFailAttempts = "1"

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

//You should pass the max_fail_attempts value as "1". 

//new Plan()
//{
//    Name = "Testing 3 Days Plan, Regular payments, no startup, no trial",
//    Description = "Testing 3 Days Plan description.",
//    Type = "infinite",
//    State = "ACTIVE",
//    PaymentDefinitions = new List<PaymentDefinition>()
//    {
//        new PaymentDefinition()
//        {
//            Name = "Testing 3 Days Plan with Regular payments, no startup, no trial, every third day charge",
//            Type = "REGULAR",
//            Frequency = "DAY",
//            FrequencyInterval = "3",
//            Amount = new PayPal.v1.BillingPlans.Currency()
//            {
//                CurrencyCode = "GBP",
//                Value = "1.00"
//            },
//            Cycles = "0",
//            ChargeModels = new List<ChargeModel>(){
//                                new ChargeModel() {
//                                   Type = "SHIPPING",
//                                   Amount = new PayPal.v1.BillingPlans.Currency()
//                                   {
//                                       CurrencyCode = "GBP",
//                                       Value = "0"
//                                   }
//                                 },
//                                new ChargeModel() {
//                                   Type = "TAX",
//                                   Amount = new PayPal.v1.BillingPlans.Currency()
//                                   {
//                                       CurrencyCode = "GBP",
//                                       Value = "0"
//                                   }
//                                 }
//            },
//        }
//    },

//    MerchantPreferences = new MerchantPreferences()
//    {
//        SetupFee = new PayPal.v1.BillingPlans.Currency()
//            {
//                CurrencyCode = "GBP",
//                Value = "1"
//            },
//        ReturnUrl = returnUrl,
//        CancelUrl = cancelUrl,
//        AutoBillAmount = "NO",
//        InitialFailAmountAction = "CANCEL",
//        MaxFailAttempts = "0"

//    }
//}