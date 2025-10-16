using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace cake_shop_back_end.Helpers;

public class Authen(string key) : IJwtAuth
{
    public string Authentication(string username, string password, string usertype)
    {
        if (!(username.Equals(username) || password.Equals(password)))
        {
            return null;
        }

        // 1. Create Security Token Handler
        var tokenHandler = new JwtSecurityTokenHandler();

        // 2. Create Private Key to Encrypted
        var tokenKey = Encoding.ASCII.GetBytes(key);

        //3. Create JETdescriptor
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
                new Claim[]
                {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.NameIdentifier, usertype)
                }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        //4. Create Token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // 5. Return Token from method
        return tokenHandler.WriteToken(token);
    }
}
