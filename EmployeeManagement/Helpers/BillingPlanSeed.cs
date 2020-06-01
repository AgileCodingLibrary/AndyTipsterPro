using PayPal.v1.BillingPlans;
using System.Collections.Generic;

namespace AndyTipsterPro.Helpers
{
    public static class BillingPlanSeed
    {


        public static List<Plan> PayPalPlans(string returnUrl, string cancelUrl) => new List<Plan>()
        {
            new Plan()
            {
                Name = "Elite Package - Monthly",
                Description = "The combination of UK/IRE & International Racing Tips Daily. (0620)",
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
                            Value = "28.00"
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
                        Value = "28.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Elite Package - 3 Months",
                Description = "The combination of UK/IRE & International Racing Tips Daily. (0620)",
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
                            Value = "70.00"
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
                        Value = "70.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Combination Package UK/IRE - Monthly",
                Description = "The UK package but with Irish Racing Tips included (0620)",
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
                            Value = "24.00"
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
                        Value = "24.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "Combination Package UK/IRE -  3 Months",
                Description = "The UK package but with Irish Racing Tips included (0620)",
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
                            Value = "60.00"
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
                        Value = "60.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "UK Racing Only Package - Monthly",
                Description = "Specialising in UK Racing Tips Daily (0620)",
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
                            Value = "19.00"
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
                        Value = "19.00"
                    },
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            },
            new Plan()
            {
                Name = "UK Racing Only Package - 3 Months",
                Description = "Specialising in UK Racing Tips Daily (0620)",
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
