namespace AutoHelper.Application.Common.Interfaces;

public interface IAesEncryptionService
{
    string Decrypt(string cipherText);
    string Encrypt(string plainText);
}