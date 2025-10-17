using System.Security.Cryptography;
using cake_shop_back_end.Models.auth;

namespace cake_shop_back_end.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // Look for any Users.
        if (context.Users.Any())
        {
            return;   // DB has been seeded
        }

        string pwd = "";
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes("12345678"));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            pwd = builder.ToString();
        }

        var userAdmin = new User { username = "administrator", status = 1, password = pwd, is_sysadmin = true, is_admin = true };
        userAdmin.date_created = DateTime.Today;
        userAdmin.date_updated = DateTime.Today;
        userAdmin.user_created = "administrator";
        userAdmin.user_updated = "administrator";
        context.Users.Add(userAdmin);
        context.SaveChanges();
    }
}
