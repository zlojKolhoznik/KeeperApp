using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class AddLoginViewModel : AddRecordViewModel
    {
        private string login;
        private string password;
        private string url;
        private string notes;

        public AddLoginViewModel(KeeperDbContext dbContext, SignInManager signInManager) : base(dbContext, signInManager)
        {
        }

        public string Login 
        {
            get => login; 
            set => SetProperty(ref login, value);
        }

        public string Password 
        { 
            get => password;
            set => SetProperty(ref password, value);
        }

        public string Url 
        { 
            get => url;
            set => SetProperty(ref url, value); 
        }

        public string Notes 
        { 
            get => notes;
            set => SetProperty(ref notes, value); 
        }

        public string LoginPlaceholder => resourceLoader.GetString("Login");
        public string PasswordPlaceholder => resourceLoader.GetString("Password");
        public string UrlPlaceholder => resourceLoader.GetString("Url");
        public string NotesPlaceholder => resourceLoader.GetString("Notes");

        protected override async Task SaveRecordAsync()
        {
            Record = new LoginRecord
            {
                Login = Login,
                Password = Password,
                Url = Url,
                Notes = Notes
            };
            await base.SaveRecordAsync();
        }

        public override bool IsInputValid()
        {
            return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(RecordTitle);
        }
    }
}
