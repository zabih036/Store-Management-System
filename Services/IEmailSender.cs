using System.Threading.Tasks;

namespace GeneralSalesDb.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
