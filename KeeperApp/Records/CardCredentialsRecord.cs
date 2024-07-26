using KeeperApp.Controls;
using KeeperApp.Records.ViewAttributes;
using KeeperApp.Security;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Records
{
    public class CardCredentialsRecord : Record
    {
        private string cardNumber;
        private string expiryDate;
        private string cvv;
        private string iconPath;

        [EncryptProperty, RequiredProperty]
        public string CardNumber
        {
            get => cardNumber;
            set
            {
                cardNumber = value;
                OnPropertyChanged();
            }
        }
     
        [EncryptProperty, RequiredProperty, ViewControl(typeof(ExpiryDateBox), "ValueProperty")]
        public string ExpiryDate
        {
            get => expiryDate;
            set
            {
                expiryDate = value;
                OnPropertyChanged();
            }
        }

        [EncryptProperty, RequiredProperty, ViewControl(typeof(CvvBox), "CvvProperty")]
        public string Cvv
        {
            get => cvv;
            set
            {
                cvv = value;
                OnPropertyChanged();
            }
        }

        [EncryptProperty, Hidden]
        public override string IconPath
        {
            get => string.IsNullOrWhiteSpace(iconPath) ? "/Assets/card.png" : iconPath;
            set
            {
                iconPath = value;
                OnPropertyChanged();
            }
        }
        
        [Hidden]
        public override string Subtitle => CardNumber;
    }
}
