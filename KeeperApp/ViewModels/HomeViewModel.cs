using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class HomeViewModel : ObservableRecipient, IRecipient<RecordMessage>
    {
        private readonly KeeperDbContext dbContext;
        private readonly ResourceLoader resourceLoader;
        private ObservableCollection<Record> records;

        public HomeViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            resourceLoader = new ResourceLoader();
            Records = new ObservableCollection<Record>(dbContext.GetRecordsForUser(signInManager.CurrentUserName).OrderByDescending(r => r.Created));
            WeakReferenceMessenger.Default.Register<HomeViewModel, RecordMessage>(this, (r, m) => r.Receive(m));
        }

        public ObservableCollection<Record> Records
        {
            get => records;
            set
            {
                records = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<Record> DeleteRecordCommand => new(DeleteRecord);

        public string Title => resourceLoader.GetString("KeeperVault");
        public string AddRecordButtonLabel => resourceLoader.GetString("AddRecord");
        public string DeleteRecordPrompt => resourceLoader.GetString("DeleteRecordPrompt");
        public string Yes => resourceLoader.GetString("Yes");
        public string No => resourceLoader.GetString("No");
        public string AddLoginRecordButtonLabel => resourceLoader.GetString("LoginRecord");
        public string AddtCardCredentialsRecordButtonLabel => resourceLoader.GetString("CardCredentialsRecord");

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
                    Records.Add(message.Record);
                    break;
                case RecordMessageType.Deleted:
                    var recordToDelete = Records.FirstOrDefault(r => r.Id == message.Record.Id);
                    Records.Remove(recordToDelete);
                    break;
                case RecordMessageType.Updated:
                    var recordIndex = Records.IndexOf(Records.FirstOrDefault(r => r.Id == message.Record.Id));
                    Records[recordIndex] = message.Record;
                    break;
            }
            Records = new ObservableCollection<Record>(Records.OrderByDescending(r => r.Created));
        }
    }
}
