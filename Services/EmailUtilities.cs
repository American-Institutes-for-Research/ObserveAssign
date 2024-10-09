using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ObserveAssign.Services
{
    public class EmailUtilities
    {
        public string toName;
        public string toEmailAddress;
        public string fromName;
        public string fromEmailAddress;
        public string subject;
        public string htmlBody;
        public string textBody;

        public EmailUtilities(string toName, string toEmail, string fromName, string fromAddress, string subject, string html, string text)
        {
            this.toName = toName;
            this.toEmailAddress = toEmail;
            this.fromName = fromName;
            this.fromEmailAddress = fromAddress;
            this.subject = subject;
            this.htmlBody = html;
            this.textBody = text;
        }

        /// <summary>
        /// Sends an email using the email settings provided
        /// Reference - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio
        /// Task Scheduler - https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler?view=net-6.0
        /// </summary>
        public string sendEmail()
        {
            //can't send email if any null values exist
            if (String.IsNullOrEmpty(this.toName) || String.IsNullOrEmpty(this.toEmailAddress) || String.IsNullOrEmpty(this.fromName)
                || String.IsNullOrEmpty(this.fromEmailAddress) || String.IsNullOrEmpty(this.subject))
            {
                return "Missing values, cannot send email.";
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(this.fromName, this.fromEmailAddress));
            message.To.Add(new MailboxAddress(this.toName, this.toEmailAddress));
            message.Subject = this.subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                //TO DO: Move this to settings files
                client.Connect("SMTP_HOST", 587, SecureSocketOptions.StartTls);

                string serverResponse = client.Send(message);
                client.Disconnect(true);
                return serverResponse;
            }
        }
    }
}
