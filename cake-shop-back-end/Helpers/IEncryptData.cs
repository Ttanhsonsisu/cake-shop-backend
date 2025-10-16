namespace cake_shop_back_end.Helpers;

public interface IEncryptData
{
    public string EncryptDataFunction(string publicKey, object dataEncrypt);
    public object DecryptDataFunction(string publicKey, string dataDecrypt);
}
