using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using KeeperApp.Security.Authentication;
using KeeperApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class FolderInfoViewModel : RecordInfoViewModel
    {
        private ObservableCollection<Record> addedRecords;
        private ObservableCollection<Record> freeRecords;
        private readonly SignInManager signInManager;
        private IUserInteractionService userIneractionManager;

        public FolderInfoViewModel(KeeperDbContext dbContext, SignInManager signInManager, IUserInteractionService userIneractionManager) : base(dbContext)
        {
            this.signInManager = signInManager;
            this.userIneractionManager = userIneractionManager;
        }

        public override void SetRecordById(int recordId)
        {
            var records = dbContext.GetRecordsForUser(signInManager.CurrentUserName);
            Record = dbContext.Folders.Find(recordId);
            AddedRecords = new ObservableCollection<Record>(records.Where(r => r.ParentId == Folder.Id));
            FreeRecords = new ObservableCollection<Record>(records.Where(r => r.ParentId != Folder.Id && r != Folder));
        }

        public AsyncRelayCommand<Record> AddRecordToFolderCommand => new(AddRecordToFolder);
        public string RecordsLabel => resourceLoader.GetString("Records");
        public string ConfirmationMessage => resourceLoader.GetString("AddToFolderConfirmation");

        private async Task AddRecordToFolder(Record record)
        {
            bool confirmed = false;
            if (record.ParentId is not null)
            {
                var recordFolderTitle = dbContext.Folders.Find(record.ParentId).Title;
                var message = string.Format(ConfirmationMessage, recordFolderTitle, Folder.Title);
                confirmed = await userIneractionManager.AskUserYesNo(message);
            }
            if (confirmed)
            {
                record.ParentId = Folder.Id;
                await dbContext.SaveChangesAsync();
                AddedRecords.Add(record);
                FreeRecords.Remove(record);
                WeakReferenceMessenger.Default.Send(new RecordMessage { Record = record, MesasgeType = RecordMessageType.Updated });
            }
        }

        public AsyncRelayCommand<Record> RemoveRecordFromFolderCommand => new(RemoveRecordFromFolder);

        private async Task RemoveRecordFromFolder(Record record)
        {
            record.ParentId = null;
            await dbContext.SaveChangesAsync();
            AddedRecords.Remove(record);
            FreeRecords.Add(record);
            WeakReferenceMessenger.Default.Send(new RecordMessage { Record = record, MesasgeType = RecordMessageType.Updated });
        }

        public Folder Folder => (Folder)Record;

        public ObservableCollection<Record> AddedRecords 
        {
            get => addedRecords;
            set => SetProperty(ref addedRecords, value);
        }

        public ObservableCollection<Record> FreeRecords
        {
            get => freeRecords; 
            set => SetProperty(ref freeRecords, value);
        }
    }
}
