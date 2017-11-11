using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace Axerrio.Identity.API.Services
{
    public class SendGridMessageService : IMessageService
    {
        public async Task Send(string email, string subject, string message)
        {
            var emailMessage = new SendGridMessage();
            emailMessage.AddTo(email);
            emailMessage.Subject = subject;
            emailMessage.From = new EmailAddress("info@axerrio.com", "info");
            emailMessage.HtmlContent = message;
            emailMessage.PlainTextContent = message;

            var client = new SendGridClient("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");

            var response = await client.SendEmailAsync(emailMessage);

            //var transportWeb = new SendGrid.Web("PUT YOUR SENDGRID KEY HERE");
            //try
            //{
            //    await transportWeb.DeliverAsync(emailMessage);
            //}
            //catch (InvalidApiRequestException ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.Errors.ToList().Aggregate((allErrors, error) => allErrors += ", " + error));
            //}
        }
    }
}
