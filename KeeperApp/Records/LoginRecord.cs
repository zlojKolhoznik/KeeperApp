using KeeperApp.Records.ViewAttributes;
using KeeperApp.Security;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Graphics.Display;

namespace KeeperApp.Records
{
    public class LoginRecord : Record
    {
        private string login;
        private string password;
        private string url;
        private string notes;
        private string iconPath;

        [EncryptProperty]
        public string Login 
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtitle));
            }
        }

        [EncryptProperty, ViewContol(typeof(PasswordBox), "PasswordProperty")]
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        [EncryptProperty]
        public string Url
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        [EncryptProperty]
        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }

        [Hidden]
        public override string Subtitle => Login;

        [EncryptProperty, Hidden]
        public override string IconPath 
        { 
            get => string.IsNullOrWhiteSpace(iconPath) ? "/Assets/record.png" : iconPath;
            set
            {
                iconPath = value;
                OnPropertyChanged();
            }
        }
    }
}
