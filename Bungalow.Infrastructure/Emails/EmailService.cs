using Microsoft.Extensions.Configuration;
using BungalowApi.Application.Contract;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WhiteLagoon.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly string _sendGridKey;
        public EmailService(IConfiguration configuration)
        {
            _sendGridKey = configuration["SendGrid:Key"];
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_sendGridKey);
            var from = new EmailAddress("info@FAArik", "FAArik - Bungalow.Web");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }
}