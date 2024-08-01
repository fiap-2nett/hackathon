using System.Threading.Tasks;
using HealthMed.Application.Core.Abstractions.Messaging;
using HealthMed.Infrastructure.Messaging.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HealthMed.Infrastructure.Messaging
{
    internal sealed class MailService : IMailService
    {
        #region Read-Only Fields

        private readonly ILogger _logger;
        private readonly MailSettings _mailSettings;

        #endregion

        #region Constructors

        public MailService(ILogger<MailService> logger, IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
        }

        #endregion

        #region IMailService Members

        public async Task SendEmailAsync(string mailTo, string subject, string textBody, string htmlBody)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Sender);
                email.To.Add(MailboxAddress.Parse(mailTo));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.TextBody = textBody;
                builder.HtmlBody = htmlBody;

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSsl);
                await smtp.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        #endregion
    }
}
