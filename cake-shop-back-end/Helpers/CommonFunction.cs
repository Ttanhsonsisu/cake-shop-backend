using System.Data;
using System.Security.Cryptography;
using cake_shop_back_end.Extensions;
using Microsoft.Data.SqlClient;

namespace cake_shop_back_end.Helpers;

public class CommonFunction(IConfiguration configuration) : ICommonFunction
{
    public string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public DateTime ConvertStringFullToDate(string stringDate)
    {
        return DateTime.ParseExact(stringDate, "dd/MM/yyyy HH:mm:tt", null);
    }

    public DateTime ConvertStringSortToDate(string stringDate)
    {
        return DateTime.ParseExact(stringDate, "dd/MM/yyyy", null);
    }

    public string ConvertDateToStringSort(DateTime? dateObject)
    {
        if (dateObject == null)
        {
            return DateTime.Now.ToString("dd/MM/yyyy");
        }
        else
        {
            DateTime dateConvert = (DateTime)dateObject;
            return dateConvert.ToString("dd/MM/yyyy");
        }
    }

    public string ConvertDateToStringFull(DateTime? dateObject)
    {
        if (dateObject == null)
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        else
        {
            DateTime dateConvert = (DateTime)dateObject;
            return dateConvert.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }

    public string ReplaceRandomStringTo(string replaceString)
    {
        if (replaceString.Length < 4)
        {
            return replaceString;
        }

        StringBuilder stringReturn = new StringBuilder(replaceString);
        for (int i = 2; i < stringReturn.Length - 2; i++)
        {
            stringReturn[i] = '*';
        }
        return stringReturn.ToString();
    }


}
