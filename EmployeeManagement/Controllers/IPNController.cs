﻿//PayPal sends IPN messages for every type of transaction or transaction status update(including payment and subscription notifications), and each notification type contains a unique set of fields.You need to configure your listener to handle the fields for every type of IPN message you might receive, depending on the types of PayPal transactions you support.For a complete guide on the different types of IPN messages and the data fields associated with each type, see the IPN Integration Guide.

//Consider the following processes in your notification handler to properly direct the notifications and to guard against fraud:


//Use the Transaction ID value to ensure you haven't already processed the notification.
//Confirm the status of the transaction, and take proper action depending on value.For example, payment response options include Completed, Pending, and Denied. Don't send inventory unless the transaction has completed!
//Validate the e-mail address of the receiver.
//Verify the item description and transaction costs with those listed on your website and catalog.
//Use the values of txn_type or reason_code of a VERIFIED notification to determine your processing actions.


using AndyTipsterPro.Entities;
using AndyTipsterPro.Helpers;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PayPal.v1.BillingAgreements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class IPNController : Controller
    {

        private readonly AppDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly PayPalHttpClientFactory _clientFactory;


        public IPNController(AppDbContext dbcontext,
                             IConfiguration configuration,
                             PayPalHttpClientFactory clientFactory)
        {
            _dbcontext = dbcontext;
            this._configuration = configuration;
            this._clientFactory = clientFactory;

        }
        private class IPNLocalContext
        {
            public HttpRequest IPNRequest { get; set; }

            public string RequestBody { get; set; }

            public string Verification { get; set; } = String.Empty;
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Receive()
        {
            try
            {
                IPNLocalContext ipnContext = new IPNLocalContext()
                {
                    IPNRequest = Request
                };

                using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII))
                {
                    ipnContext.RequestBody = reader.ReadToEnd();
                }

                //Store the IPN received from PayPal
                await LogAndEmailRequest(ipnContext);

                //React backend as per IPN Received
                await UpdateSubscription(ipnContext);

                //Fire and forget verification task
                VerifyTask(ipnContext);

                //Reply back a 200 code
                return Ok();
            }
            catch (Exception)
            {

                using (Stream Body = Request.Body)
                {
                    byte[] result;
                    result = new byte[Request.Body.Length];
                    await Request.Body.ReadAsync(result, 0, (int)Request.Body.Length);

                    String body = System.Text.Encoding.UTF8.GetString(result).TrimEnd('\0');

                    PaypalErrors error = new PaypalErrors
                    {
                        Exception = body,
                        DateTime = DateTime.Now
                    };

                    _dbcontext.PaypalErrors.Add(error);
                    _dbcontext.SaveChanges();

                    await EmailSuperAdmin(body, "Receive method failed");
                }

                await EmailSuperAdmin("Body", "Receive method failed, did you get the body");
                return Ok();
            }


        }

        private async Task UpdateSubscription(IPNLocalContext ipn)
        {

            if (ipn != null && ipn.RequestBody != null)
            {
                var response = ipn.RequestBody;
                var keys = response.Split('&');
                var data = new Dictionary<string, string>();
                foreach (var key in keys)
                {
                    //payment_type = instant
                    var field = key.Split('=');
                    data.Add(field[0], field[1]);
                }


                //cancel Reversed transactions.
                if (data.ContainsKey("payment_status"))
                {
                    if (data["payment_status"] == "Reversed" || data["payment_status"] == "Canceled_Reversal")
                    {
                        //update database
                        var payPalAgreement = data["recurring_payment_id"];

                        if (!String.IsNullOrEmpty(payPalAgreement))
                        {
                            await ReversedPaymentFound(payPalAgreement);
                            await TellPayPalToCancelSubscription(payPalAgreement);
                        }
                    }

                   
                }


                if (data["txn_type"] == "recurring_payment_profile_created")
                {
                    //A new customer tried to subscribed but their payment failed. Delete their subscription on the website and on PayPal side.
                    if (data.ContainsKey("initial_payment_status"))
                    {
                        if (data["initial_payment_status"] == "Failed")
                        {
                            //update database
                            var payPalAgreement = data["recurring_payment_id"];

                            if (!String.IsNullOrEmpty(payPalAgreement))
                            {
                                await NewSubscriptionFirstPaymentFailedDeleteSubscription(payPalAgreement);
                                await TellPayPalToCancelSubscription(payPalAgreement);
                            }

                        }

                        //A new customer has just successfully subscribed.
                        if (data["initial_payment_status"] == "Completed")
                        {
                            //update database for start date.
                            var payPalAgreement = data["recurring_payment_id"];

                            if (!String.IsNullOrEmpty(payPalAgreement))
                            {
                                await NewSubscriptionUpdateStartDate(payPalAgreement);
                            }
                            else
                            {
                                var type = data["txn_type"];
                                var status = data["initial_payment_status"];
                                await EmailSuperAdmin($"Null Agreement: IPN Type: {type}  Payment Status : {status}", "Null Agreement");
                            }
                        }

                        //A new customer tried to subscribed but their payment is PENDING. Delete their subscription on the website and on PayPal side.
                        if (data["initial_payment_status"] == "Pending")
                        {
                            //update database
                            var payPalAgreement = data["recurring_payment_id"];

                            if (!String.IsNullOrEmpty(payPalAgreement))
                            {
                                await NewSubscriptionFirstPaymentPendingDeleteSubscription(payPalAgreement);
                                await TellPayPalToCancelSubscription(payPalAgreement);
                            }
                        }
                    }

                }

                ////A new customer has just successfully subscribed.
                //if (data["txn_type"] == "recurring_payment_profile_created")
                //{
                //    if (data.ContainsKey("initial_payment_status"))
                //    {
                //        if (data["initial_payment_status"] == "Completed")
                //        {
                //            //update database for start date.
                //            var payPalAgreement = data["recurring_payment_id"];

                //            if (!String.IsNullOrEmpty(payPalAgreement))
                //            {
                //                await NewSubscriptionUpdateStartDate(payPalAgreement);
                //            }
                //            else
                //            {
                //                var type = data["txn_type"];
                //                var status = data["initial_payment_status"];
                //                await EmailSuperAdmin($"Null Agreement: IPN Type: {type}  Payment Status : {status}", "Null Agreement");
                //            }
                //        }
                //    }
                //}

                ////A new customer tried to subscribed but their payment is PENDING. Delete their subscription on the website and on PayPal side.
                //if (data["txn_type"] == "recurring_payment_profile_created")
                //{
                //    if (data.ContainsKey("initial_payment_status"))
                //    {
                //        if (data["initial_payment_status"] == "Pending")
                //        {
                //            //update database
                //            var payPalAgreement = data["recurring_payment_id"];

                //            if (!String.IsNullOrEmpty(payPalAgreement))
                //            {
                //                await NewSubscriptionFirstPaymentPendingDeleteSubscription(payPalAgreement);
                //                await TellPayPalToCancelSubscription(payPalAgreement);
                //            }
                //        }
                //    }
                //}

                //An existing customer has just renewed their subscription.
                if (data["txn_type"] == "recurring_payment")
                {
                    if (data.ContainsKey("payment_status"))
                    {
                        if (data["payment_status"] == "Completed")
                        {
                            //update database for start date.
                            var payPalAgreement = data["recurring_payment_id"];

                            if (!String.IsNullOrEmpty(payPalAgreement))
                            {
                                await NewSubscriptionUpdateStartDate(payPalAgreement);
                            }
                        }
                    }
                }

                //An existing customer has cancelled. Either Update or delete Subscription               

                if (data["txn_type"] == "recurring_payment_profile_cancel")
                {

                    if ((!data.ContainsKey("initial_payment_status")))
                    {
                        if ((data.ContainsKey("profile_status")))
                        {
                            if (data["profile_status"] == "Cancelled")
                            {
                                var payPalAgreement = data["recurring_payment_id"];

                                //update profile and allow access until expired.
                                if (!String.IsNullOrEmpty(payPalAgreement))
                                {
                                    await ExistingSubscriptionHasbeenCancelledUpdateSubscription(payPalAgreement);
                                }
                                else
                                {
                                    var type = data["txn_type"];
                                    var status = data["initial_payment_status"];
                                    await EmailSuperAdmin($"Null Agreement: IPN Type: {type}  Payment Status : {status}", "Update failed due to Null Agreement");
                                }
                            }
                        }
                    }

                    if ((data.ContainsKey("initial_payment_status")))
                    {
                        if (data["initial_payment_status"] == "Failed")
                        {
                            var payPalAgreement = data["recurring_payment_id"];

                            //ACTION : Remove subscription completly.
                            if (!String.IsNullOrEmpty(payPalAgreement))
                            {
                                await ExistingSubscriptionPaymentFailedUpdateSubscription(payPalAgreement);
                            }
                            else
                            {
                                var type = data["txn_type"];
                                var status = data["initial_payment_status"];
                                await EmailSuperAdmin($"Null Agreement: IPN Type: {type}  Payment Status : {status}", "Delete failed due to Null Agreement");
                            }
                        }
                    }

                }

                //An existing customer has DENIED their payment. DEELTE Subscription.
                if ((data["txn_type"] == "recurring_payment") && (data["payment_status"] == "Denied"))
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];

                    if (!String.IsNullOrEmpty(payPalAgreement))
                    {
                        await ExistingSubscriptionPaymentHasbeenDeniedUpdateSubscription(payPalAgreement);
                        await TellPayPalToCancelSubscription(payPalAgreement);
                    }
                }


                //An existing customer has FAILED 3 PAYMENTS. Delete Subscription.                  
                if (data["txn_type"] == "recurring_payment_failed")
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];

                    if (!String.IsNullOrEmpty(payPalAgreement))
                    {
                        await ExistingSubscriptionPaymentFailedUpdateSubscription(payPalAgreement);
                        await TellPayPalToCancelSubscription(payPalAgreement);
                    }
                }


                //An existing customer has FAILED 3 PAYMENTS. Delete Subscription. 
                if (data["txn_type"] == "recurring_payment_suspended_due_to_max_failed_payment")
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];

                    if (!String.IsNullOrEmpty(payPalAgreement))
                    {
                        await ExistingSubscriptionPaymentFailedMaxFailedPaymentsUpdateSubscription(payPalAgreement);
                        await TellPayPalToCancelSubscription(payPalAgreement);
                    }
                }

                //An existing customer has SKIPPED their payment. NO ACTION REQUIRED AT THE MOMENT..
                if (data["txn_type"] == "recurring_payment_skipped")
                {

                    // NO ACTION REQURIED.

                }


                //txn_type = new_case
                //CASE CREATED- (They’ve created a case to get there money back, these also can be instantly removed)                  
                if (data["txn_type"] == "new_case")
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];

                    if (!String.IsNullOrEmpty(payPalAgreement))
                    {
                        await CaseCreatedDeleteSubscription(payPalAgreement);
                        await TellPayPalToCancelSubscription(payPalAgreement);
                    }
                }

            }
        }


        private async Task NewSubscriptionUpdateStartDate(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
            if (userSubscription != null)
            {
                userSubscription.StartDate = DateTime.Now;
                _dbcontext.UserSubscriptions.Update(userSubscription);
                await _dbcontext.SaveChangesAsync();
            }
        }

        private async Task TellPayPalToCancelSubscription(string payPalAgreement)
        {
            try
            {
                var client = _clientFactory.GetClient();

                var requestForPayPalAgreement = new AgreementGetRequest(payPalAgreement);
                var result = await client.Execute(requestForPayPalAgreement);
                var agreement = result.Result<Agreement>();

                var request = new AgreementCancelRequest(payPalAgreement).RequestBody(new AgreementStateDescriptor()
                {
                    Note = "Cancelled"
                });

                await client.Execute(request);

                var message = $"PayPal has been notified to cancel Subscription :{agreement.Id} for the package : {agreement.Description} under {agreement.Name}.";
                var subject = $"PayPal has been notified to Cancel Subscription :{agreement.Id}";
                await EmailAdmin(message, subject);

                //await EmailSuperAdmin("Notify PayPal to Cancel Subscription SUCCESS", "Notify PayPal to Cancel Subscription SUCCESS");

            }
            catch (Exception ex)
            {

                // save error in the database.
                PaypalErrors payPalReturnedError = new PaypalErrors()
                {
                    Exception = ex.Message,
                    DateTime = DateTime.Now

                };

                _dbcontext.PaypalErrors.Add(payPalReturnedError);
                await _dbcontext.SaveChangesAsync();

                await EmailSuperAdmin($"Notify PayPal ({payPalAgreement}) to Cancel Subscription Failed", "Notify PayPal to Cancel Subscription Failed");
            }

        }

        private async Task NewSubscriptionFirstPaymentFailedDeleteSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement && !String.IsNullOrEmpty(x.PayPalAgreementId)).FirstOrDefault();
            if (userSubscription != null)
            {
                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();


                await EmailCustomer(userSubscription.PayerEmail,
                                    "Thanks for trying to Subscribe at AndyTipster. However your payment failed. Please try again.",
                                    "Payment for AndyTipster Failed");


                var message = $"A new Subscription, FIRST PAYMENT FAILED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have failed their payment. They have no Access to this subscription.";

                await EmailAdmin(message, "A new Subscription, FIRST PAYMENT FAILED, Subscription DELETED.");
            }
        }

        private async Task NewSubscriptionFirstPaymentPendingDeleteSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement && !String.IsNullOrEmpty(x.PayPalAgreementId)).FirstOrDefault();
            if (userSubscription != null)
            {
                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();


                await EmailCustomer(userSubscription.PayerEmail,
                                    "Thanks for trying to Subscribe at AndyTipster. However your payment is still PENDING. Please try again.",
                                    "Payment for AndyTipster Pending");


                var message = $"A new Subscription, FIRST PAYMENT PENDING: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} first payment is have PENDING.. They have no Access to this subscription.";

                await EmailAdmin(message, "A new Subscription, FIRST PAYMENT PENDING, Subscription Cancelled.");
            }
        }

        private async Task ExistingSubscriptionHasbeenCancelledUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement && !String.IsNullOrEmpty(x.PayPalAgreementId)).FirstOrDefault();
            if (userSubscription != null)
            {

                //get user subscription
                var client = _clientFactory.GetClient();
                AgreementGetRequest request = new AgreementGetRequest(userSubscription.PayPalAgreementId);
                BraintreeHttp.HttpResponse result = await client.Execute(request);
                Agreement agreement = result.Result<Agreement>();

                string expiryDateAsString = agreement.AgreementDetails.NextBillingDate.Substring(0, 10);
                DateTime expiryDate = DateTime.ParseExact(expiryDateAsString, "yyyy-MM-dd", null);

                userSubscription.ExpiryDate = expiryDate;
                userSubscription.State = "Cancelled";
                _dbcontext.UserSubscriptions.Update(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled. You will have access until {userSubscription.ExpiryDate}.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled");

                var adminMessage = $"Regular Subscription CANCELLED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have CANCELLED {userSubscription.Description}. They will have access until : {userSubscription.ExpiryDate}.";

                await EmailAdmin(adminMessage, "Regular Subscription CANCELLED, Subscription UPDATED.");
            }
        }

        private async Task ExistingSubscriptionPaymentHasbeenDeniedUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement && !String.IsNullOrEmpty(x.PayPalAgreementId)).FirstOrDefault();
            if (userSubscription != null)
            {

                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled due to DENIED payment.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled due to DENIED payment.");

                var adminMessage = $"Regular Subscription PAYMENT DENIED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have DENIED PAYMENT {userSubscription.Description}. Subscription has been removed.";

                await EmailAdmin(adminMessage, "Regular Subscription DENIED PAYMENT, Subscription REMOVED.");
            }
        }


        private async Task ReversedPaymentFound(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement && !String.IsNullOrEmpty(x.PayPalAgreementId)).FirstOrDefault();
            if (userSubscription != null)
            {

                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled due to REVERSED or Canceled_Reversal payment.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled due to REVERSED or Canceled_Reversal payment.");

                var adminMessage = $"Regular Subscription PAYMENT REVERSED or Canceled_Reversal: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have REVERSED or Canceled_Reversal PAYMENT {userSubscription.Description}. Subscription has been removed.";

                await EmailAdmin(adminMessage, "Regular Subscription REVERSED or Canceled_Reversal PAYMENT, Subscription REMOVED.");
            }
        }

        private async Task ExistingSubscriptionPaymentFailedUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
            if (userSubscription != null)
            {

                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled due to FAILED payments.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled due to FAILED payments.");

                var adminMessage = $"Regular Subscription PAYMENTS FAILED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have FAILED PAYMENTs {userSubscription.Description}. Subscription has been removed.";

                await EmailAdmin(adminMessage, "Regular Subscription FAILED PAYMENT, Subscription REMOVED.");
            }
        }

        private async Task ExistingSubscriptionPaymentFailedMaxFailedPaymentsUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
            if (userSubscription != null)
            {

                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled due to FAILED payments.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled due to FAILED payments.");

                var adminMessage = $"recurring_payment_suspended_due_to_max_failed_payment: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have recurring_payment_suspended_due_to_max_failed_payment {userSubscription.Description}. Subscription has been removed.";

                await EmailAdmin(adminMessage, "recurring_payment_suspended_due_to_max_failed_payment, Subscription REMOVED.");
            }
        }

        private async Task ExistingSubscriptionPaymentSkippedUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement && !String.IsNullOrEmpty(x.PayPalAgreementId)).FirstOrDefault();
            if (userSubscription != null)
            {

                //get user subscription
                var client = _clientFactory.GetClient();
                AgreementGetRequest request = new AgreementGetRequest(userSubscription.PayPalAgreementId);
                BraintreeHttp.HttpResponse result = await client.Execute(request);
                Agreement agreement = result.Result<Agreement>();

                string expiryDateAsString = agreement.AgreementDetails.NextBillingDate.Substring(0, 10);
                DateTime expiryDate = DateTime.ParseExact(expiryDateAsString, "yyyy-MM-dd", null);

                userSubscription.ExpiryDate = expiryDate;
                userSubscription.State = "Cancelled";
                _dbcontext.UserSubscriptions.Update(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled due to SKIPPED payment. You will have access until {userSubscription.ExpiryDate}.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled due to SKIPPED payment.");

                var adminMessage = $"Regular Subscription PAYMENT SKIPPED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have SKIPPED PAYMENT {userSubscription.Description}. They will have access until : {userSubscription.ExpiryDate}.";

                await EmailAdmin(adminMessage, "Regular Subscription SKIPPED PAYMENT, Subscription UPDATED.");
            }
        }

        private async Task CaseCreatedDeleteSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
            if (userSubscription != null)
            {

                _dbcontext.UserSubscriptions.Remove(userSubscription);
                await _dbcontext.SaveChangesAsync();

                var userMessage = $"Your subscription for {userSubscription.Description} has been cancelled due to FAILED payments.";
                await EmailCustomer(userSubscription.PayerEmail, userMessage, "AndyTipster subscription has been cancelled due to CASE CREATED payments.");

                var adminMessage = $"Regular Subscription PAYMENTS FAILED and CASE CREATED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
                              $": {userSubscription.PayerEmail} have FAILED PAYMENTs and CASE CREATED {userSubscription.Description}. Subscription has been removed.";

                await EmailAdmin(adminMessage, "Regular Subscription FAILED PAYMENT and CASE CREATED, Subscription REMOVED.");
            }
        }


        private void VerifyTask(IPNLocalContext ipnContext)
        {
            try
            {
                //var verificationRequest = System.Net.WebRequest.Create("https://ipnpb.sandbox.paypal.com/cgi-bin/webscr");

                var verificationRequest = System.Net.WebRequest.Create("https://ipnpb.paypal.com/cgi-bin/webscr");

                //Send response messages back to PayPal:

                //https://ipnpb.sandbox.paypal.com/cgi-bin/webscr (for Sandbox IPNs)
                //https://ipnpb.paypal.com/cgi-bin/webscr (for live IPNs)

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";

                //Add cmd=_notify-validate to the payload
                string strRequest = "cmd=_notify-validate&" + ipnContext.RequestBody;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII))
                {
                    writer.Write(strRequest);
                }

                //Send the request to PayPal and get the response
                using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream()))
                {
                    ipnContext.Verification = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

                PaypalErrors error = new PaypalErrors
                {
                    Exception = ex.Message,
                    DateTime = DateTime.Now
                };

                _dbcontext.PaypalErrors.Add(error);
                _dbcontext.SaveChanges();

            }

            ProcessVerificationResponse(ipnContext);
        }

        private async Task LogAndEmailRequest(IPNLocalContext ipnContext)
        {
            var sendEmailsAndLog = true;

            // Persist the request values into a database or temporary data store
            IPNContext ipn = new IPNContext
            {
                RequestBody = ipnContext.RequestBody,
                Verification = ipnContext.Verification
            };


            if (ipn != null && ipn.RequestBody != null)
            {
                var response = ipn.RequestBody;
                var keys = response.Split('&');
                var data = new Dictionary<string, string>();
                foreach (var key in keys)
                {
                    //payment_type = instant
                    var field = key.Split('=');
                    data.Add(field[0], field[1]);

                }


                if (data["txn_type"] == "recurring_payment_profile_created")
                {
                    if (data.ContainsKey("initial_payment_status"))
                    {
                        if (data["initial_payment_status"] == "Completed")
                        {
                            //do not send email
                            sendEmailsAndLog = true;
                        }
                    }
                }

                if (data["txn_type"] == "recurring_payment")
                {
                    if (data.ContainsKey("payment_status"))
                    {
                        if (data["payment_status"] == "Completed")
                        {
                            //do not send email
                            sendEmailsAndLog = true;
                        }
                    }
                }

                //An existing customer has decided to cancel. Set Expiry date on the subscription.
                if (data["txn_type"] == "recurring_payment_profile_cancel")
                {
                    //do not send email
                    sendEmailsAndLog = true;

                }

                //An existing customer has SKIPPED their payment. NO ACTION REQUIRED AT THE MOMENT..
                if (data["txn_type"] == "recurring_payment_skipped")
                {
                    //do not send email
                    sendEmailsAndLog = true;

                }


                //A new customer tried to subscribed but their payment is PENDING. Delete their subscription on the website and on PayPal side.
                if (data["txn_type"] == "recurring_payment_profile_created")
                {
                    if (data.ContainsKey("initial_payment_status"))
                    {
                        if (data["initial_payment_status"] == "Pending")
                        {
                            //do not send email
                            sendEmailsAndLog = true;
                        }
                    }
                }

                if (sendEmailsAndLog)
                {
                    //send email
                    var message = BuildEmailMessage(ipn);
                    var subject = BuildEmailSubject(ipn);
                    await EmailAdmin(message, subject);

                    _dbcontext.IPNContexts.Add(ipn);
                    _dbcontext.SaveChanges();
                }

            }
        }

        private async Task EmailAdmin(string message, string subject)
        {
            //notify Me, when this gets.
            var sendGridKey = _configuration.GetValue<string>("SendGridApi");
            //await Emailer.SendEmail("fazahmed786@hotmail.com", subject, message, sendGridKey);
            await Emailer.SendEmail("andytipster99@gmail.com", subject, message, sendGridKey);
        }

        private async Task EmailSuperAdmin(string message, string subject)
        {
            //notify Me, when this gets.
            var sendGridKey = _configuration.GetValue<string>("SendGridApi");
            await Emailer.SendEmail("fazahmed786@hotmail.com", subject, message, sendGridKey);
        }

        private async Task EmailCustomer(string emailAddress, string message, string subject)
        {
            //notify Me, when this gets.
            var sendGridKey = _configuration.GetValue<string>("SendGridApi");
            await Emailer.SendEmail(emailAddress, subject, message, sendGridKey);
        }

        private string BuildEmailSubject(IPNContext ipn)
        {
            var subject = "IPN Notification: ";

            if (ipn != null && ipn.RequestBody != null)
            {
                var response = ipn.RequestBody;

                var keys = response.Split('&');


                var data = new Dictionary<string, string>();

                foreach (var key in keys)
                {
                    //payment_type = instant
                    var field = key.Split('=');
                    data.Add(field[0], field[1]);
                }

                var firstName = data["first_name"];
                var lastName = data["last_name"];
                var email = data["payer_email"].Replace("%40", "@");

                subject = $"{subject} : {firstName} {lastName} : {email}";
            }

            return subject;

        }

        private string BuildEmailMessage(IPNContext ipn)
        {
            var message = "";

            if (ipn != null && ipn.RequestBody != null)
            {
                var response = ipn.RequestBody;

                var keys = response.Split('&');


                var data = new Dictionary<string, string>();

                foreach (var key in keys)
                {
                    //payment_type = instant
                    var field = key.Split('=');
                    data.Add(field[0], field[1]);
                    var param = field[0] + " :  " + field[1] + Environment.NewLine;
                    message += String.Concat(param);
                }



                var firstName = data["first_name"];
                var lastName = data["last_name"];
                var email = data["payer_email"].Replace("%40", "@");

                var subject = $"{firstName} {lastName} : {email}";




                ////check the transaction type
                //// recurring_payment_profile_created
                //// recurring_payment
                //// recurring_payment_failed
                //// recurring_payment_profile_cancel
                //// recurring_payment_suspended_due_to_max_failed_payment

                //var transactionType = data["txn_type"];

                //var transactionAlert = "";
                //var paymentStatus = "";
                //var amountPaid = "";
                //var currency = "";
                //var payPalTransactionId = "";
                //var product = "";
                //var business = "";

                //if (transactionType == "recurring_payment_profile_created")
                //{
                //    transactionAlert = "A new customer recurring payment profile has been created.";
                //    paymentStatus = data["initial_payment_status"].ToUpper();
                //    amountPaid = data["initial_payment_amount"];
                //    currency = data["currency_code"];
                //    payPalTransactionId = data["initial_payment_txn_id"];
                //    product = data["product_name"].Replace("+", " ");
                //}
                //else if (transactionType == "recurring_payment")
                //{
                //    transactionAlert = "An existing customer has paid a recurring payment.";
                //    paymentStatus = data["payment_status"].ToUpper();
                //    amountPaid = data["mc_gross"];
                //    currency = data["mc_currency"];
                //    payPalTransactionId = data["txn_id"];
                //    product = data["transaction_subject"].Replace("+", " ");
                //    business = data["business"];
                //}
                //else if (transactionType == "recurring_payment_failed")
                //{
                //    transactionAlert = "A recurring payment has failed";
                //}
                //else if (transactionType == "recurring_payment_profile_cancel")
                //{
                //    transactionAlert = "A recurring payment profile has been cancelled.";
                //}
                //else if (transactionType == "recurring_payment_suspended_due_to_max_failed_payment")
                //{
                //    transactionAlert = "A recurring payment has been suspended due to max failed payments.";
                //}
                //else
                //{
                //    transactionAlert = "unknown event.";
                //}

                //var paymentDate = DateTime.Now;

                //var payerId = data["payer_id"];
                //var payerFirstName = data["first_name"];
                //var payerLastName = data["last_name"];
                //var payerEmail = data["payer_email"].Replace("%40", "@");

                //var buyerEmail = data["receiver_email"].Replace("%40", "@");
                //var pendingReason = "Not pending";

                //if (paymentStatus == "Pending")
                //{
                //    pendingReason = data["pending_reason"];
                //}


                //message = $"{transactionAlert} A Payment of {amountPaid} recieved with status of {paymentStatus}. (TransactionId: {payPalTransactionId} ). " +
                //    $"Payer {payerFirstName} {payerLastName} with PayPal Email {payerEmail} has PayPal Id {payerId}. " +
                //    $"Payment of {amountPaid} is for product: {product} " +
                //    $"Amount {amountPaid} will go to PayPal Email {buyerEmail}.";



            }

            return message;
        }

        private void ProcessVerificationResponse(IPNLocalContext ipnContext)
        {
            if (ipnContext.Verification.Equals("VERIFIED"))
            {
                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment
            }
            else if (ipnContext.Verification.Equals("INVALID"))
            {
                //Log for manual investigation
            }
            else
            {
                //Log error
            }
        }

    }
}