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
            //notify Me, when this gets.
            var sendGridKey = _configuration.GetValue<string>("SendGridApi");            
            await Emailer.SendEmail("fazahmed786@hotmail.com", "IPN Received from PayPal ",  Request.Body.ToString(), sendGridKey);

            IPNLocalContext ipnContext = new IPNLocalContext()
            {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII))
            {
                ipnContext.RequestBody = reader.ReadToEnd();
            }

            //Store the IPN received from PayPal
            LogRequest(ipnContext);

            //Fire and forget verification task
            //Task.Run(() => VerifyTask(ipnContext));

            await VerifyTask(ipnContext);

            //Reply back a 200 code
            return Ok();
        }

        private async Task VerifyTask(IPNLocalContext ipnContext)
        {
            try
            {
                var verificationRequest = System.Net.WebRequest.Create("https://ipnpb.paypal.com/cgi-bin/webscr");

                //var verificationRequest = System.Net.WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

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
               await _dbcontext.SaveChangesAsync();

            }

            ProcessVerificationResponse(ipnContext);
        }


        private void LogRequest(IPNLocalContext ipnContext)
        {
            // Persist the request values into a database or temporary data store

            IPNContext ipn = new IPNContext
            {
                RequestBody = ipnContext.RequestBody,
                Verification = ipnContext.Verification
            };

            _dbcontext.IPNContexts.Add(ipn);
            _dbcontext.SaveChanges();
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