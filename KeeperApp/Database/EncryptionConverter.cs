using KeeperApp.Security;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Database
{
    public class EncryptionConverter : ValueConverter
    {
        public EncryptionConverter() : base((object v) => v == null ? v : AesEncryptor.Encrypt(v.ToString()),
            (object v) => v == null ? v : AesEncryptor.Decrypt(v.ToString()), null)
        {
            ConvertToProvider = v => v is null ? v : AesEncryptor.Encrypt(v.ToString());
            ConvertFromProvider = v => v is null ? v : AesEncryptor.Decrypt(v.ToString());
        }

        public override Func<object, object> ConvertToProvider { get; }

        public override Func<object, object> ConvertFromProvider { get; }

        public override Type ModelClrType => typeof(string);

        public override Type ProviderClrType => typeof(string);
    }
}
