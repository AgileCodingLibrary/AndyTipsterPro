using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AndyTipsterPro.Helpers
{
    public static class Emailer
    {

        public static async Task SendEmail(string email, string subject, string htmlContent, string sendGridKey)
        {
            var apiKey = sendGridKey;

            var client = new SendGridClient(apiKey);            
            var from = new EmailAddress("CustomerSupport@AndyTipster.com", "Support");
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(htmlContent, "<[^>]*>", "");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public static async Task SendBroadCastEmail(string email, string subject, string htmlContent, string sendGridKey)
        {
            var apiKey = sendGridKey;

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("CustomerSupport@AndyTipster.com", "Andy Tipster");
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(htmlContent, "<[^>]*>", "");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
