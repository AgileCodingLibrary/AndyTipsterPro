using PayPal.v1.BillingPlans;
using System.Collections.Generic;

namespace EmployeeManagement.Helpers
{
    public static class BillingPlanSeed
    {


        public static List<Plan> PayPalPlans(string returnUrl, string cancelUrl) => new List<Plan>()
        {
            new Plan()
            {
                Name = "Andy Tipster Package - Monthly",
                Description = "Specialising in UK Racing and American Sports",
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
                            CurrencyCode = "GBP",
                            Value = "15.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "GBP",
                        Value = "15.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Andy Tipster Package - 3 Months",
                Description = "Specialising in UK Racing and American Sports",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "40.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "GBP",
                        Value = "40.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Irish Horse Racing - Monthly",
                Description = "Solely Irish Horse Racing Tips",
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
                            CurrencyCode = "GBP",
                            Value = "13.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "GBP",
                        Value = "13.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Irish Horse Racing -  3 Months",
                Description = "Solely Irish Horse Racing Tips",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "35.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "GBP",
                        Value = "35.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Ultimate pack - Monthly",
                Description = "Enjoy The 2 Brands For Less, The ultimate pack for the ultimate Deal",
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
                            CurrencyCode = "GBP",
                            Value = "19.99"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "GBP",
                        Value = "19.99"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Ultimate pack - 3 Months",
                Description = "Enjoy The 2 Brands For Less, The ultimate pack for the ultimate Deal",
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
                        Amount = new Currency()
                        {
                            CurrencyCode = "GBP",
                            Value = "50.00"
                        },
                        Cycles = "0"
                    }
                },
                MerchantPreferences = new MerchantPreferences()
                {
                    // The initial payment
                    SetupFee = new Currency()
                    {
                        CurrencyCode = "GBP",
                        Value = "50.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            }


        };

    }
}
