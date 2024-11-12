namespace ShareResource.Interfaces
{
    public interface IEncryptionService
    {
        string EncryptData(string plainText);
        string DecryptData(string cipherText);

    }
}
