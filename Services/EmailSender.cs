using Microsoft.Extensions.Options;
using GeneralSalesDb.Extensions;
using System.Threading.Tasks;

namespace GeneralSalesDb.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> options)
        {
            this.options = options.Value;
        }
        public AuthMessageSenderOptions options { get; }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
            // return Execute("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", subject, message, email);
        }
        //public Task Execute(string apikey,string subject,string message,string email)
        //{
        //    //var client = new SendGridClient(apikey);
        //    //var msg = new SendGridMessage()
        //    //{
        //    //    From = new EmailAddress("zabih036@gmail.com", "pharmacy"),
        //    //    Subject = subject,
        //    //    PlainTextContent = message,
        //    //    HtmlContent = message
        //    //};
        //    //msg.AddTo(new EmailAddress(email));
        //    //msg.SetClickTracking(false, false);
        //    //return client.SendEmailAsync(msg);
        //}
    }
}
