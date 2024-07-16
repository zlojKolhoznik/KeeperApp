using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using KeeperApp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class AddRecordViewModel : ObservableObject, IValidator
    {
        private readonly KeeperDbContext dbContext;
        private readonly SignInManager signInManager;
        private string title;
        private string login;
        private string password;
        private string url;
        private string notes;

        public string Title
        {
            get => title;
            set 
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string Password 
        { 
            get => password; 
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public string Url
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }

        public AddRecordViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
        }

        public RelayCommand SaveRecordCommand => new(SaveRecord);

        private void SaveRecord()
        {
            if (IsInputValid())
            {
                var record = new LoginRecord
                {
                    Title = Title,
                    Login = Login,
                    Password = Password,
                    Url = Url,
                    Notes = Notes,
                    Created = DateTime.Now,
                    Modified = DateTime.MinValue,
                    IsDeleted = false
                };
                record.OwnerUsernameHash = Sha256Hasher.GetSaltedHash(signInManager.CurrentUserName, record.Created.ToString());
                dbContext.Logins.Add(record);
                dbContext.SaveChanges();
                var message = new RecordMessage
                {
                    Record = record,
                    MesasgeType = RecordMessageType.Added
                };
                WeakReferenceMessenger.Default.Send(message);
            }
        }

        public bool IsInputValid()
        {
            return !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        }
    }
}
