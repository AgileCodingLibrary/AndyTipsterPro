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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class IPNController : Controller
    {

        private readonly AppDbContext _dbcontext;
        private readonly IConfiguration _configuration;


        public IPNController(AppDbContext dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            this._configuration = configuration;

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

            //Fire and forget verification task
            //Task.Run(() => VerifyTask(ipnContext));

            VerifyTask(ipnContext);

            //Reply back a 200 code
            return Ok();
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
            // Persist the request values into a database or temporary data store

            IPNContext ipn = new IPNContext
            {
                RequestBody = ipnContext.RequestBody,
                Verification = ipnContext.Verification
            };

            _dbcontext.IPNContexts.Add(ipn);
            _dbcontext.SaveChanges();

            //send email
            var message = BuildEmailMessage(ipn);
            var subject = BuildEmailSubject(ipn);
            await EmailAdmin(message, subject);
        }

        private async Task EmailAdmin(string message, string subject)
        {
            //notify Me, when this gets.
            var sendGridKey = _configuration.GetValue<string>("SendGridApi");
            await Emailer.SendEmail("fazahmed786@hotmail.com", subject, message, sendGridKey);
            await Emailer.SendEmail("a.thorndyke@hotmail.co.uk", subject, message, sendGridKey);
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