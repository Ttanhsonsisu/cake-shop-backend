namespace cake_shop_back_end.Extensions;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendListEmailAsync(List<string> emails, string subject, string message);
    Task SendSms(string phone_number, string message);
}
