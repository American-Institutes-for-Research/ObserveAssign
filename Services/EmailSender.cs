using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.RegularExpressions;

namespace ObserveAssign.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string textBody = Regex.Replace(htmlMessage, "<.*?>", string.Empty);
            if (htmlMessage.Contains("href"))//manually handle links
            {
                textBody = htmlMessage.Replace("<a href='", "clicking here: ").Replace("'>clicking here</a>", "");
            }

            EmailUtilities newEmail = new EmailUtilities(email, email, "ObserveAssign", "noreply@DOMAIN", subject, htmlMessage, textBody);

            return Task.FromResult(newEmail.sendEmail());
        }
    }
}
