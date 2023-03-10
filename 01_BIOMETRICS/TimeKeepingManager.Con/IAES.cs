namespace TimeKeepingManager.Con.Security
{
    public interface IAES
    {
        string Decrypt(string ciphertext, string key);
        string Encrypt(string plainText, string key);
    }
}
