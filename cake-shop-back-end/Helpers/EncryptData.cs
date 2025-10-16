using System.Security.Cryptography;
using Newtonsoft.Json;

namespace cake_shop_back_end.Helpers;

public class EncryptData : IEncryptData
{
    public string EncryptDataFunction(string publicKey, object dataEncrypt)
    {
        string stringEncrypt = JsonConvert.SerializeObject(dataEncrypt);
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(publicKey);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(stringEncrypt);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public object DecryptDataFunction(string publicKey, string dataDecrypt)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(dataDecrypt);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(publicKey);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        string stringReturn = streamReader.ReadToEnd();
                        object dataReturn = JsonConvert.DeserializeObject(stringReturn);
                        return dataReturn;
                    }
                }
            }
        }
    }

    //public string EncryptDataFunction(string publicKey, object dataEncrypt)
    //{
    //    string stringEncrypt = JsonConvert.SerializeObject(dataEncrypt);
    //    byte[] plainTextBytes = Encoding.UTF8.GetBytes(stringEncrypt);
    //    string stringDecode = Convert.ToBase64String(plainTextBytes);
    //    CspParameters CSApars = new CspParameters();
    //    CSApars.KeyContainerName = publicKey;

    //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSApars);

    //    byte[] byteText = Encoding.UTF8.GetBytes(stringDecode);
    //    byte[] byteEntry = rsa.Encrypt(byteText, false);

    //    return Convert.ToBase64String(byteEntry);
    //}


    //public object DecryptDataFunction(string publicKey, string dataDecrypt)
    //{
    //    CspParameters CSApars = new CspParameters();
    //    CSApars.KeyContainerName = publicKey;

    //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSApars);

    //    byte[] byteEntry = Convert.FromBase64String(dataDecrypt);
    //    byte[] byteText = rsa.Decrypt(byteEntry, false);

    //    string base64String = Encoding.UTF8.GetString(byteText);
    //    byte[] plainTextBytes = Convert.FromBase64String(base64String);
    //    string stringDecryptJson = Encoding.UTF8.GetString(plainTextBytes);
    //    object dataReturn = JsonConvert.DeserializeObject(stringDecryptJson);

    //    return dataReturn;
    //}

}
