using KeeperApp.Controls;
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

        [EncryptProperty, RequiredProperty]
        public string Login 
        {
            get => login;
            set
            {
                SetProperty(ref login, value);
                OnPropertyChanged(nameof(Subtitle));
            }
        }

        [EncryptProperty, RequiredProperty, ViewControl(typeof(PasswordBoxWithGenerator), "PasswordProperty")]
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        [EncryptProperty]
        public string Url
        {
            get => url;
            set => SetProperty(ref url, value);
        }

        [EncryptProperty]
        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        [Hidden]
        public override string Subtitle => Login;

        [EncryptProperty, Hidden]
        public override string IconPath 
        { 
            get => string.IsNullOrWhiteSpace(iconPath) ? "/Assets/person.png" : iconPath;
            set => SetProperty(ref iconPath, value);
        }
    }
}
