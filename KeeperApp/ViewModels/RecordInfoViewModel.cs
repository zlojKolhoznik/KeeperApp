using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using System;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public abstract class RecordInfoViewModel : ObservableRecipient, IRecipient<RecordMessage>
    {
        private bool readOnlyMode;
        protected readonly KeeperDbContext dbContext;
        protected Record record;
        protected readonly ResourceLoader resourceLoader;

        public RecordInfoViewModel(KeeperDbContext dbContext)
        {
            this.dbContext = dbContext;
            resourceLoader = new ResourceLoader();
            WeakReferenceMessenger.Default.Register<RecordInfoViewModel, RecordMessage>(this, (r, m) => r.Receive(m));
        }

        public string Save => resourceLoader.GetString("Save");
        public string Cancel => resourceLoader.GetString("Cancel");
        public string TitleLabel => resourceLoader.GetString("Title");

        public Record Record
        {
            get => record;
            set => SetProperty(ref record, value, nameof(Record));
        }

        public bool ReadOnlyMode
        {
            get => readOnlyMode;
            set => SetProperty(ref readOnlyMode, value, nameof(ReadOnlyMode));
        }

        public RelayCommand EnterEditingModeCommand => new(() => ReadOnlyMode = false);

        public RelayCommand SaveChangesCommand => new(() =>
        {
            Record.Modified = DateTime.Now;
            dbContext.Update(Record);
            dbContext.SaveChanges();
            WeakReferenceMessenger.Default.Send(new RecordMessage { Record = Record, MesasgeType = RecordMessageType.Updated });
            ReadOnlyMode = true;
        });

        public RelayCommand CancelChangesCommand => new(() =>
        {
            dbContext.Entry(Record).Reload();
            WeakReferenceMessenger.Default.Send(new RecordMessage { Record = Record, MesasgeType = RecordMessageType.Updated });
            ReadOnlyMode = true;
        });

        public abstract void SetRecordById(int recordId);

        public void Receive(RecordMessage message)
        {
            if (message.MesasgeType == RecordMessageType.Updated && message.Record.Id == Record.Id)
            {
                OnPropertyChanged(nameof(Record));
            }
        }
    }
}
