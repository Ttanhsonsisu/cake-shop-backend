namespace cake_shop_back_end.DataObjects.Requests.Auth;

public class PasswordRequest
{
    public string? OtpCode { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
    public int? UserId { get; set; }
}
