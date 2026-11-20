using CarRental.Application.DTOs.Auth;
using CarRental.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CarRental.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingsDTO _settings;

        public EmailService(IOptions<EmailSettingsDTO> options)
        {
            _settings = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string body)
        {
            return SendAsync(email, subject, body, null);
        }

        public Task SendEmailWithAttachmentAsync(
            string email,
            string subject,
            string body,
            string attachmentFileName,
            byte[] attachmentContent,
            string attachmentContentType = "application/pdf")
        {
            return SendAsync(
                email,
                subject,
                body,
                builder => builder.Attachments.Add(
                    attachmentFileName,
                    attachmentContent,
                    ContentType.Parse(attachmentContentType)));
        }

        private async Task SendAsync(
            string email,
            string subject,
            string body,
            Action<BodyBuilder>? configure)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _settings.DisplayName,
                    _settings.Email));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            configure?.Invoke(builder);

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.Email,
                _settings.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}