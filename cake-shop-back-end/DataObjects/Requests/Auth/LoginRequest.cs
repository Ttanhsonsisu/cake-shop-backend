namespace cake_shop_back_end.DataObjects.Requests.Auth;

public class LoginRequest
{
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? OtpCode { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ShareCode { get; set; }
    //public Boolean? is_android { get; set; }
    public string? DeviceId { get; set; }
}
