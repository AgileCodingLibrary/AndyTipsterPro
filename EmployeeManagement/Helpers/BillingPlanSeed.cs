﻿using PayPal.v1.BillingPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Helpers
{
    public static class BillingPlanSeed
    {


        public static List<Plan> PayPalPlans(string returnUrl, string cancelUrl) => new List<Plan>()
        {
            new Plan()
            {
                Name = "Just Browsing Plan",
                Description = "1 great new beer sent to your door each month.",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "USD",
                            Value = "5.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "USD",
                        Value = "5.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },

            new Plan()
            {
                Name = "Let's Do This Plan",
                Description = "A refreshing 6-pack of assorted beers delivered to your door each month.",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "USD",
                            Value = "24.95"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "USD",
                        Value = "24.95"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },

            new Plan()
            {
                Name = "Beard Included Plan",
                Description =
                    "A hand picked carton of the most delicious and rare beers placed delicately on your doorstep each month.",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "USD",
                            Value = "59.95"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "USD",
                        Value = "59.95"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },

            new Plan()
            {
                Name = "Hook It To My Veins Plan",
                Description =
                    "Angels whisper sweet nothings into your ears as 48 precious glass bottles are carefully packed into your fridge each month.",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "USD",
                            Value = "100.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "USD",
                        Value = "100.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            }
        };

    }
}
