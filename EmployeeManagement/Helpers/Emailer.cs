using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmployeeManagement.Helpers
{
    public static class Emailer
    {

        public static async Task SendEmail(string email, string subject, string htmlContent, string sendGridKey)
        {
            var apiKey = sendGridKey;

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("fazahmed786@hotmail.com", "Support");
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(htmlContent, "<[^>]*>", "");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
