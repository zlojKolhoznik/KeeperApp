using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Models;
using KeeperApp.Records;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class HomeViewModel : ObservableRecipient, IRecipient<RecordMessage>
    {
        private readonly KeeperDbContext dbContext;
        private readonly ResourceLoader resourceLoader;
        private RecordTree records;

        public HomeViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            resourceLoader = new ResourceLoader();
            var records = dbContext.GetRecordsForUser(signInManager.CurrentUserName).OrderByDescending(r => r.Created);
            Records = new RecordTree(records);
            Records.StructureChanged += (p, r) =>
            {
                r.ParentId = p?.Id;
                dbContext.SaveChanges();
            };
            WeakReferenceMessenger.Default.Register<HomeViewModel, RecordMessage>(this, (r, m) => r.Receive(m));
        }

        public RelayCommand<Record> DeleteRecordCommand => new(DeleteRecord);

        public string Title => resourceLoader.GetString("KeeperVault");
        public string AddRecordButtonLabel => resourceLoader.GetString("AddRecord");
        public string DeleteRecordPrompt => resourceLoader.GetString("DeleteRecordPrompt");
        public string Yes => resourceLoader.GetString("Yes");
        public string No => resourceLoader.GetString("No");
        public string AddLoginRecordButtonLabel => resourceLoader.GetString("LoginRecord");
        public string AddtCardCredentialsRecordButtonLabel => resourceLoader.GetString("CardCredentialsRecord");
        public string AddFolderButtonLabel => resourceLoader.GetString("Folder");

        public RecordTree Records
        {
            get => records; 
            set => SetProperty(ref records, value); 
        }

        public void DeleteRecord(Record record)
        {
            dbContext.Remove(record);
            dbContext.SaveChanges();
            WeakReferenceMessenger.Default.Send(new RecordMessage { Record = record, MesasgeType = RecordMessageType.Deleted });
        }

        public void Receive(RecordMessage message)
        {
            switch (message.MesasgeType)
            {
                case RecordMessageType.Added:
                    Records.Insert(message.Record);
                    break;
                case RecordMessageType.Deleted:
                    Records.Remove(message.Record);
                    break;
                case RecordMessageType.Updated:
                    Records.UpdateLocation(message.Record);
                    break;
            }
        }
    }
}
