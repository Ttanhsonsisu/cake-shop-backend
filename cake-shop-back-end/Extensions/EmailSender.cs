using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace cake_shop_back_end.Extensions;

public class EmailSender(IConfiguration _configuration) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var gmailSection = _configuration.GetSection("EmailSenderConfig:Gmail");
        string smtpServer = gmailSection["smtpServer"];
        string nameEmail = gmailSection["NameEmail"];
        string password = gmailSection["Password"];

        var email_send = new MimeMessage();

        email_send.From.Add(MailboxAddress.Parse("ttanhsonsisu@gmail.com"));

        email_send.To.Add(MailboxAddress.Parse(email));
        email_send.Subject = subject;
        email_send.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

        // send email
        using var smtp = new SmtpClient();

        smtp.Connect(smtpServer, 465, SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(nameEmail.ToString() , password.ToString());

        smtp.Send(email_send);
        smtp.Disconnect(true);
        return Task.CompletedTask;
    }

    public Task SendListEmailAsync(List<string> emails, string subject, string message)
    {
        if (emails.Count == 0)
        {
            return Task.CompletedTask;
        }
        var email_send = new MimeMessage();
        email_send.From.Add(MailboxAddress.Parse("12323.123213@12312323.vn"));
        for (int i = 0; i < emails.Count; i++)
        {
            email_send.To.Add(MailboxAddress.Parse(emails[i]));
        }
        email_send.Subject = subject;
        email_send.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

        // send email
        using var smtp = new SmtpClient();
        smtp.Connect("SMTP.YANDEX.COM", 465, SecureSocketOptions.SslOnConnect);
        smtp.Authenticate("123123123noreply@12323123.vn", "12312312312");
        smtp.Send(email_send);
        smtp.Disconnect(true);
        return Task.CompletedTask;
    }

    public Task SendSms(string phone_number, string message)
    {
        throw new NotImplementedException();
    }

    private class TokenOTPResponse
    {
        public string? CodeResult;
        public string? CountRegenerate;
        public string? SMSID;
    }
}
