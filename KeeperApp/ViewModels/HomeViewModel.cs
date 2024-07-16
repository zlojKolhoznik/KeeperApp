using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class HomeViewModel : ObservableRecipient, IRecipient<RecordMessage>
    {
        private readonly KeeperDbContext dbContext;
        private readonly SignInManager signInManager;
        private ObservableCollection<Record> records;

        public HomeViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
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
            }
            Records = new ObservableCollection<Record>(Records.OrderByDescending(r => r.Created));
        }
    }
}
