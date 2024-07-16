using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class EditRecordViewModel : ObservableObject, IValidator
    {
        private readonly KeeperDbContext dbContext;
        private string title;
        private string login;
        private string password;
        private string url;
        private string notes;
        private LoginRecord record;

        public LoginRecord Record
        {
            get => record;
            set
            {
                record = value;
                Title = record.Title;
                Login = record.Login;
                Password = record.Password;
                Url = record.Url;
                Notes = record.Notes;
            }
        }

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

        public EditRecordViewModel(KeeperDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public RelayCommand SaveEditingCommang => new(SaveEditing);

        private void SaveEditing()
        {
            if (IsInputValid())
            {

                record.Title = Title;
                record.Login = Login;
                record.Password = Password;
                record.Url = Url;
                record.Notes = Notes;
                dbContext.Logins.Update(record);
                dbContext.SaveChanges();
                var message = new RecordMessage { MesasgeType = RecordMessageType.Updated, Record = record };
                WeakReferenceMessenger.Default.Send(message);
            }
        }

        public bool IsInputValid()
        {
            return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
