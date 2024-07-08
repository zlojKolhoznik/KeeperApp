using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Graphics.Display;

namespace KeeperApp.Records
{
    public class LoginRecord : Record, IParsable<LoginRecord>
    {
        private string login;
        private string password;
        private string url;
        private string notes;

        public string Login 
        { 
            get => Decrypt<string>(login);
            set
            {
                login = Encrypt(value);
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => Decrypt<string>(password);
            set
            {
                password = Encrypt(value);
                OnPropertyChanged();
            }
        }

        public string Url
        {
            get => Decrypt<string>(url);
            set
            {
                url = Encrypt(value);
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get => Decrypt<string>(notes);
            set
            {
                notes = Encrypt(value);
                OnPropertyChanged();
            }
        }

        public static LoginRecord Parse(string s, IFormatProvider provider)
        {
            return JsonSerializer.Deserialize<LoginRecord>(s);
        }

        public static LoginRecord Parse(string s)
        {
            return Parse(s, null);
        }

        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out LoginRecord result)
        {
            bool returnValue;
            try
            {
                result = Parse(s, provider);
                returnValue = true;
            }
            catch
            {
                result = null;
                returnValue = false;
            }

            return returnValue;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
