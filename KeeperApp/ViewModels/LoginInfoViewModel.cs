using KeeperApp.Database;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class LoginInfoViewModel : RecordInfoViewModel
    {
        public LoginInfoViewModel(KeeperDbContext dbContext) : base(dbContext)
        {
        }

        public new LoginRecord Record
        {
            get => (LoginRecord)record;
            set => SetProperty(ref record, value, nameof(Record));
        }

        public string LoginLabel => resourceLoader.GetString("Login");
        public string PasswordLabel => resourceLoader.GetString("Password");
        public string UrlLabel => resourceLoader.GetString("Url");
        public string NotesLabel => resourceLoader.GetString("Notes");

        public override void SetRecordById(int recordId)
        {
            Record = dbContext.Logins.Find(recordId);
        }
    }
}
