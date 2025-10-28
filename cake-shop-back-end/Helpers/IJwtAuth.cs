namespace cake_shop_back_end.Helpers;

public interface IJwtAuth
{
    string Authentication(string username, string password, string userType);
    string AuthenticationStore(string username, string password, string userType, Guid id);
}
