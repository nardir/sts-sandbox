using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;

namespace Axerrio.Identity.API.Services
{
    //https://github.com/sendgrid/sendgrid-csharp/blob/master/USE_CASES.md#singleemailsinglerecipient
    public class SendGridMessageService : IMessageService
    {
        private readonly SendGridMessageOptions _options;

        public SendGridMessageService(IOptions<SendGridMessageOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }
        public async Task Send(string email, string subject, string message)
        {
            var emailMessage = new SendGridMessage();
            emailMessage.AddTo(email);
            emailMessage.Subject = subject;
            emailMessage.From = new EmailAddress("info@axerrio.com", "info");
            emailMessage.HtmlContent = message;
            emailMessage.PlainTextContent = message;

            var client = new SendGridClient(_options.SendGridApiKey);

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
