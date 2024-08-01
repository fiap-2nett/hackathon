using System.Threading.Tasks;

namespace HealthMed.Application.Core.Abstractions.Messaging
{
    public interface IMailService
    {
        #region IMailService Members

        Task SendEmailAsync(string mailTo, string subject, string textBody, string htmlBody);

        #endregion
    }
}
