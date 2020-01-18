using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BillingPlansController : Controller
    {
        //public ActionResult Index()
        //{
        //    var apiContext = GetApiContext();

        //    var list = PayPal.Api.Plan.List(apiContext, status: "ACTIVE");

        //    if (list == null || !list.plans.Any())
        //    {
        //        SeedBillingPlans(apiContext);
        //        list = PayPal.Api.Plan.List(apiContext, status: "ACTIVE");
        //    }

        //    return View(list);
        //}

        ////public ActionResult Delete(string id)
        ////{
        ////    var apiContext = GetApiContext();

        ////    var plan = new Plan()
        ////    {
        ////        id = id
        ////    };

        ////    plan.Delete(apiContext);

        ////    return RedirectToAction("Index");
        ////}

        //public ActionResult DeleteAll() 
        //{
        //    var apiContext = GetApiContext();

        //    var list = PayPal.Api.Plan.List(apiContext, status: "ACTIVE");

        //    foreach (var plan in list.plans)
        //    {
        //        var deletePlan = new Plan()
        //        {
        //            id = plan.id
        //        };

        //        deletePlan.Delete(apiContext);
        //    }

        //    return RedirectToAction("Index");
        //}

        ///// <summary>
        ///// Create the default billing plans for this example website
        ///// </summary>
        //private void SeedBillingPlans(APIContext apiContext)
        //{
        //    //1. Create Paypal plan objects, using static Paypal class Plan.
        //    var AndyTipsterMonthlyPackage = new Plan()
        //    {
        //        name = "Andy Tipster Package - Monthly Subscription",
        //        description = "Andy Tipster Package, Monthly Subscription, Specialising in UK Racing.",
        //        type = "infinite",
        //        payment_definitions = new List<PaymentDefinition>()
        //        {
        //            new PaymentDefinition()
        //            {
        //                name = "Regular Payments",
        //                type = "REGULAR",
        //                frequency = "MONTH",
        //                frequency_interval = "1",
        //                amount = new Currency()
        //                {
        //                    currency = "GBP",
        //                    value = "13.00"
        //                },
        //                cycles = "0"
        //            }
        //        },
        //        merchant_preferences = new MerchantPreferences()
        //        {
        //            // The initial payment
        //            setup_fee = new Currency()
        //            {
        //                currency = "GBP",
        //                value = "13.00"
        //            },
        //            return_url = Url.Action("Return", "Subscription", null, Request.GetEncodedUrl()),
        //            cancel_url = Url.Action("Cancel", "Subscription", null, Request.GetEncodedUrl())
        //        }
        //    };
        //    var AndyTipster3MonthsPackage = new Plan()
        //    {
        //        name = "Andy Tipster Package - Three Months Subscription",
        //        description = "Andy Tipster Package, Three Months Subscription, Specialising in UK Racing.",
        //        type = "infinite",
        //        payment_definitions = new List<PaymentDefinition>()
        //        {
        //            new PaymentDefinition()
        //            {
        //                name = "Regular Payments",
        //                type = "REGULAR",
        //                frequency = "MONTH",
        //                frequency_interval = "3",
        //                amount = new Currency()
        //                {
        //                    currency = "GBP",
        //                    value = "35.00"
        //                },
        //                cycles = "0"
        //            }
        //        },
        //        merchant_preferences = new MerchantPreferences()
        //        {
        //            // The initial payment
        //            setup_fee = new Currency()
        //            {
        //                currency = "GBP",
        //                value = "35.00"
        //            },
        //            return_url = Url.Action("Return", "Subscription", null, Request.GetEncodedUrl()),
        //            cancel_url = Url.Action("Cancel", "Subscription", null, Request.GetEncodedUrl())
        //        }
        //    };

        //    var IrishHorseRacingMonthlyPackage = new Plan()
        //    {
        //        name = "Irish Horse Racing - Monthly Subscription",
        //        description = "Irish Horse Racing, Monthly Subscription, Solely Irish Horse Racing Tips",
        //        type = "infinite",
        //        payment_definitions = new List<PaymentDefinition>()
        //        {
        //            new PaymentDefinition()
        //            {
        //                name = "Regular Payments",
        //                type = "REGULAR",
        //                frequency = "MONTH",
        //                frequency_interval = "1",
        //                amount = new Currency()
        //                {
        //                    currency = "GBP",
        //                    value = "10.00"
        //                },
        //                cycles = "0"
        //            }
        //        },
        //        merchant_preferences = new MerchantPreferences()
        //        {
        //            // The initial payment
        //            setup_fee = new Currency()
        //            {
        //                currency = "GBP",
        //                value = "10.00"
        //            },
        //            return_url = Url.Action("Return", "Subscription", null, Request.GetEncodedUrl()),
        //            cancel_url = Url.Action("Cancel", "Subscription", null, Request.GetEncodedUrl())
        //        }
        //    };
        //    var IrishHorseRacing3MonthsPackage = new Plan()
        //    {
        //        name = "Irish Horse Racing - Three Months Subscription",
        //        description = "Irish Horse Racing, Three Months Subscription, Solely Irish Horse Racing Tips",
        //        type = "infinite",
        //        payment_definitions = new List<PaymentDefinition>()
        //        {
        //            new PaymentDefinition()
        //            {
        //                name = "Regular Payments",
        //                type = "REGULAR",
        //                frequency = "MONTH",
        //                frequency_interval = "3",
        //                amount = new Currency()
        //                {
        //                    currency = "GBP",
        //                    value = "27.50"
        //                },
        //                cycles = "0"
        //            }
        //        },
        //        merchant_preferences = new MerchantPreferences()
        //        {
        //            // The initial payment
        //            setup_fee = new Currency()
        //            {
        //                currency = "GBP",
        //                value = "27.50"
        //            }
        //            ,
        //            return_url = Url.Action("Return", "Subscription", null, Request.GetEncodedUrl()),
        //            cancel_url = Url.Action("Cancel", "Subscription", null, Request.GetEncodedUrl())
        //        }
        //    };

        //    var UltimatepackMonthlyPackage = new Plan()
        //    {
        //        name = "Ultimate pack - Monthly Subscription",
        //        description = "Ultimate pack, Enjoy The 2 Brands For Less, The ultimate pack for the ultimate Deal",
        //        type = "infinite",
        //        payment_definitions = new List<PaymentDefinition>()
        //        {
        //            new PaymentDefinition()
        //            {
        //                name = "Regular Payments",
        //                type = "REGULAR",
        //                frequency = "MONTH",
        //                frequency_interval = "1",
        //                amount = new Currency()
        //                {
        //                    currency = "GBP",
        //                    value = "17.00"
        //                },
        //                cycles = "0"
        //            }
        //        },
        //        merchant_preferences = new MerchantPreferences()
        //        {
        //            // The initial payment
        //            setup_fee = new Currency()
        //            {
        //                currency = "GBP",
        //                value = "17.00"
        //            }
        //            ,
        //            return_url = Url.Action("Return", "Subscription", null, Request.GetEncodedUrl()),
        //            cancel_url = Url.Action("Cancel", "Subscription", null, Request.GetEncodedUrl())
        //        }
        //    };
        //    var Ultimatepack3MonthPackage = new Plan()
        //    {
        //        name = "Ultimate pack - Three Months Subscription",
        //        description = "Ultimate pack, Enjoy The 2 Brands For Less, The ultimate pack for the ultimate Deal",
        //        type = "infinite",
        //        payment_definitions = new List<PaymentDefinition>()
        //        {
        //            new PaymentDefinition()
        //            {
        //                name = "Regular Payments",
        //                type = "REGULAR",
        //                frequency = "MONTH",
        //                frequency_interval = "3",
        //                amount = new Currency()
        //                {
        //                    currency = "GBP",
        //                    value = "45.00"
        //                },
        //                cycles = "0"
        //            }
        //        },
        //        merchant_preferences = new MerchantPreferences()
        //        {
        //            // The initial payment
        //            setup_fee = new Currency()
        //            {
        //                currency = "GBP",
        //                value = "45.00"
        //            }
        //            ,
        //            return_url = Url.Action("Return", "Subscription", null, Request.GetEncodedUrl()),
        //            cancel_url = Url.Action("Cancel", "Subscription", null, Request.GetEncodedUrl())
        //        }
        //    };

        //    //2. Create a plan inside PayPal using Paypal context, configured inside web.config.
        //    AndyTipsterMonthlyPackage = Plan.Create(apiContext, AndyTipsterMonthlyPackage);
        //    AndyTipster3MonthsPackage = Plan.Create(apiContext, AndyTipster3MonthsPackage);
        //    IrishHorseRacingMonthlyPackage = Plan.Create(apiContext, IrishHorseRacingMonthlyPackage);
        //    IrishHorseRacing3MonthsPackage = Plan.Create(apiContext, IrishHorseRacing3MonthsPackage);
        //    UltimatepackMonthlyPackage = Plan.Create(apiContext, UltimatepackMonthlyPackage);
        //    Ultimatepack3MonthPackage = Plan.Create(apiContext, Ultimatepack3MonthPackage);

        //    //3. Create an object that will update an existing Paypal plan.
        //    //   in this case we will set an existing Paypal plan state to ACTIVE.
        //    var updateStatePatchRequest = new PatchRequest()
        //    {
        //        new Patch()
        //        {
        //            op = "replace",
        //            path = "/",
        //            value = new Plan
        //            {
        //                state = "ACTIVE"
        //            }
        //        }
        //    };

        //    //4. Update existing Paypal plan and make it ACTIVE. Plan cant have a payer if Plan is not active.
        //    AndyTipsterMonthlyPackage.Update(apiContext, updateStatePatchRequest);
        //    AndyTipster3MonthsPackage.Update(apiContext, updateStatePatchRequest);
        //    IrishHorseRacingMonthlyPackage.Update(apiContext, updateStatePatchRequest);
        //    IrishHorseRacing3MonthsPackage.Update(apiContext, updateStatePatchRequest);
        //    UltimatepackMonthlyPackage.Update(apiContext, updateStatePatchRequest);
        //    Ultimatepack3MonthPackage.Update(apiContext, updateStatePatchRequest);

        //}

        //private APIContext GetApiContext()
        //{
        //    // Authenticate with PayPal
        //    var config = ConfigManager.Instance.GetProperties();
        //    var accessToken = new OAuthTokenCredential(config).GetAccessToken();
        //    var apiContext = new APIContext(accessToken);
        //    return apiContext;
        //}

    }
}