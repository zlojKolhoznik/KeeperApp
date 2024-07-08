using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Records;

public class Record : ObservableObject
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    protected byte[] IV { get; set; }

    public Record()
    {
        Created = DateTime.Now;
        Modified = DateTime.Now;
        IsDeleted = false;
    }

    protected T Decrypt<T>(string value) where T : IParsable<T>
    {
        var key = GetEncryptionKey();
        using var aes = Aes.Create();
        var decryptor = aes.CreateDecryptor(key, IV);
        var bytes = Convert.FromBase64String(value);
        var decryptedBytes = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        return T.Parse(Encoding.UTF8.GetString(decryptedBytes), null);
    }

    protected string Encrypt<T>(T value) where T : IParsable<T>
    {
        var key = GetEncryptionKey();
        using var aes = Aes.Create();
        if (IV is null or { Length: 0 })
        {
            IV = aes.IV;
        }
        var encryptor = aes.CreateEncryptor(key, IV);
        var bytes = Encoding.UTF8.GetBytes(value.ToString());
        var encryptedBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        return Convert.ToBase64String(encryptedBytes);
    }

    private byte[] GetEncryptionKey()
    {
        var bytes = Encoding.UTF8.GetBytes(Created.ToString());
        return SHA256.HashData(bytes);
    }
}
