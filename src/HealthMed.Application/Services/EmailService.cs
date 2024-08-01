using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string doctorName)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var fromAddress = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]);
            var toAddress = new MailAddress(toEmail);
            string fromPassword = smtpSettings["Password"];
            string subject = "Nova Consulta Agendada";
            string body = $"Olá, Dr. {doctorName},\n\nUma nova consulta foi agendada.";

            var smtp = new SmtpClient
            {
                Host = smtpSettings["Server"],
                Port = int.Parse(smtpSettings["Port"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }

            _logger.LogInformation("E-mail enviado com sucesso para {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail para {ToEmail}", toEmail);            
        }
    }
}


