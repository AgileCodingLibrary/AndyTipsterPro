//PayPal sends IPN messages for every type of transaction or transaction status update(including payment and subscription notifications), and each notification type contains a unique set of fields.You need to configure your listener to handle the fields for every type of IPN message you might receive, depending on the types of PayPal transactions you support.For a complete guide on the different types of IPN messages and the data fields associated with each type, see the IPN Integration Guide.

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


                //A new customer tried to subscribed but their payment failed. Delete their subscription.
                if ((data["txn_type"] == "recurring_payment_profile_created") && (data["initial_payment_status"] == "Failed"))
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];
                    await NewSubscriptionFirstPaymentFailedDeleteSubscription(payPalAgreement);

                }

                //An existing customer has decided to cancel. Set Expiry date on the subscription.
                if (data["txn_type"] == "recurring_payment_profile_cancel")
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];
                    await ExistingSubscriptionHasbeenCancelledUpdateSubscription(payPalAgreement);

                }

                //An existing customer has DENIED their payment. DEELTE Subscription.
                if ((data["txn_type"] == "recurring_payment") && (data["payment_status"] == "Denied"))
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];
                    await ExistingSubscriptionPaymentHasbeenDeniedUpdateSubscription(payPalAgreement);
                }


                //An existing customer has FAILED 3 PAYMENTS. Delete Subscription.                
                if (data["txn_type"] == "recurring_payment_failed")
                {
                    //update database
                    var payPalAgreement = data["recurring_payment_id"];
                    await ExistingSubscriptionPaymentFailedUpdateSubscription(payPalAgreement);
                }

                //An existing customer has SKIPPED their payment. NO ACTION REQUIRED AT THE MOMENT..
                if (data["txn_type"] == "recurring_payment_skipped")
                {

                    // NO ACTION REQURIED.

                    //update database
                    //var payPalAgreement = data["recurring_payment_id"];
                    //await ExistingSubscriptionPaymentSkippedUpdateSubscription(payPalAgreement);
                }

            }
        }

     
        private async Task NewSubscriptionFirstPaymentFailedDeleteSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
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

        private async Task ExistingSubscriptionHasbeenCancelledUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
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
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
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

        private async Task ExistingSubscriptionPaymentSkippedUpdateSubscription(string payPalAgreement)
        {
            //get a user with PayPal agreement.
            var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
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
                            sendEmailsAndLog = false;
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
                            sendEmailsAndLog = false;
                        }
                    }
                }


                //An existing customer has decided to cancel. Set Expiry date on the subscription.
                if (data["txn_type"] == "recurring_payment_profile_cancel")
                {
                    //do not send email
                    sendEmailsAndLog = false;

                }

                //An existing customer has SKIPPED their payment. NO ACTION REQUIRED AT THE MOMENT..
                if (data["txn_type"] == "recurring_payment_skipped")
                {
                    //do not send email
                    sendEmailsAndLog = false;

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
            await Emailer.SendEmail("fazahmed786@hotmail.com", subject, message, sendGridKey);
            await Emailer.SendEmail("andytipster99@gmail.com", subject, message, sendGridKey);
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

   
        //private async Task UpdateDeniedPayment(string payPalAgreement)
        //{
        //    //get a user with PayPal agreement.
        //    var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
        //    if (userSubscription != null)
        //    {
        //        userSubscription.State = "Payment Denied";
        //        await _dbcontext.SaveChangesAsync();

        //        var message = $"PAYMENT DENIED: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
        //                      $": {userSubscription.PayerEmail} have denied their payment. " +
        //                      $"Access to account has been suspended.";
        //        await EmailAdmin(message, "PAYMENT DENIED");
        //    }
        //}

        //private async Task UpdateSkippedPayment(string payPalAgreement)
        //{
        //    //get a user with PayPal agreement.
        //    var userSubscription = _dbcontext.UserSubscriptions.Where(x => x.PayPalAgreementId == payPalAgreement).FirstOrDefault();
        //    if (userSubscription != null)
        //    {
        //        userSubscription.State = "Payment Skipped";
        //        await _dbcontext.SaveChangesAsync();

        //        var message = $"SKIPPED PAYMENT: {userSubscription.PayerFirstName}  {userSubscription.PayerLastName} " +
        //                      $": {userSubscription.PayerEmail} have skipped their payment. " +
        //                      $"Access to account has been suspended.";
        //        await EmailAdmin(message, "PAYMENT SKIPPED");
        //    }

        //}
    }
}