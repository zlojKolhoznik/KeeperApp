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
    public class AddFolderViewModel : AddRecordViewModel
    {
        public AddFolderViewModel(KeeperDbContext dbContext, SignInManager signInManager) : base(dbContext, signInManager)
        {
        }

        public override bool IsInputValid() => !string.IsNullOrWhiteSpace(RecordTitle);

        protected override Task SaveRecordAsync()
        {
            Record = new Folder();
            return base.SaveRecordAsync();
        }
    }
}
