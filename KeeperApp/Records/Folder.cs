using KeeperApp.Records.ViewAttributes;
using KeeperApp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Records
{
    public class Folder : Record
    {
        private string iconPath;

        [EncryptProperty, Hidden]
        public override string IconPath 
        {
            get => string.IsNullOrWhiteSpace(iconPath) ? "/Assets/folder.png" : iconPath;
            set => SetProperty(ref iconPath, value);
        }

        [Hidden]
        public override string Subtitle => string.Empty;
    }
}
