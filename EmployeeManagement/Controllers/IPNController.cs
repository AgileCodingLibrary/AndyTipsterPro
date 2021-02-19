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
                var verificationRequest = System.Net.WebRequest.Create("https://ipnpb.sandbox.paypal.com/cgi-bin/webscr");

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
            await EmailAdmin(message);
        }

        private async Task EmailAdmin(string message)
        {
            //notify Me, when this gets.
            var sendGridKey = _configuration.GetValue<string>("SendGridApi");
            await Emailer.SendEmail("fazahmed786@hotmail.com", "IPN Notification",message, sendGridKey);
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
                }

                var paymentDate = DateTime.Now;
                var paymentStatus = data["payment_status"];

                var payerId = data["payer_id"];
                var payerFirstName = data["first_name"];
                var payerLastName = data["last_name"];
                var payerEmail = data["payer_email"];

                var amountPaid = data["mc_gross"];
                var currency = data["mc_currency"];
                var payPalTransactionId = data["txn_id"];
                var transactionType = data["txn_type"];
                var product = data["item_name"];

                var business = data["business"];
                var buyerEmail = data["receiver_email"];
                var pendingReason = "Not pending";

                if (paymentStatus == "Pending")
                {
                    pendingReason = data["pending_reason"];
                }

                //message = $"Payment recieved at {paymentDate} for Transaction type ({transactionType}) and transaction Id:{payPalTransactionId} Product:{product}. Payer {payerFirstName} {payerLastName} who's PayPalId is {payerId}) and PayPal email is {payerEmail} has made a payment of {amountPaid} in {currency}. Payment status is {paymentStatus}. If status is Pending, reason is {pendingReason}. Payment of {amountPaid} will go to business {business} and email {buyerEmail}. Payment Verification : {ipn.Verification} ";

                //var sb = new StringBuilder();
                //sb.Append($"Payment Notification has been received from PayPal. ");
                //sb.AppendLine($"TransactionId: {payPalTransactionId}");
                //sb.AppendLine($"Payment For: {product}");
                //sb.AppendLine($" ");
                //sb.AppendLine($" ");
                //sb.AppendLine($"Payment status: {paymentStatus}");
                //sb.AppendLine($" ");
                //sb.AppendLine($" ");               
                //sb.AppendLine($"P A Y M E N T ");
                //sb.AppendLine($"Payment Amount: {amountPaid}");
                //sb.AppendLine($"Payment Currency: {currency}");
                //sb.AppendLine($" ");
                //sb.AppendLine($" ");
                //sb.AppendLine($"B U Y E R ");
                //sb.AppendLine($"Buyer Name: {payerFirstName} {payerLastName}");
                //sb.AppendLine($"Buyer Email: {payerEmail}");
                //sb.AppendLine($"Buyer PayPalId:  {payerId} ");
                //sb.AppendLine($" ");
                //sb.AppendLine($" ");


                //message = sb.ToString();

                message = $"Payment recieved at {paymentDate} for Transaction type ({transactionType})" + Environment.NewLine +
                    $" and transaction Id:{payPalTransactionId} Product:{product}. " + Environment.NewLine +
                    $"Payer {payerFirstName} {payerLastName} who's PayPalId is {payerId}) and " + Environment.NewLine +
                    $"PayPal email is {payerEmail} has made a payment of {amountPaid} in {currency}. Payment status is {paymentStatus}. If status is Pending, reason is {pendingReason}. Payment of {amountPaid} will go to business {business} and email {buyerEmail}. Payment Verification : {ipn.Verification} ";


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