using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace KeyManager.MailService;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly Connection _connection;
    private readonly Credentials _credentials;

    public EmailService(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
        _connection = _options.Connection;
        _credentials = _options.Credentials;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        mimeMessage.To.Add(new MailboxAddress(string.Empty, email));
        mimeMessage.Subject = subject;

        mimeMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = message
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_connection.Host, _connection.Port, _connection.IsUseSsl);
        await client.AuthenticateAsync(_credentials.Login, _credentials.ApplicationPassword);
        await client.SendAsync(mimeMessage);

        await client.DisconnectAsync(true);
    }
}