using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Models;
using KeeperApp.Records;
using KeeperApp.Security.Authentication;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class HomeViewModel : ObservableRecipient, IRecipient<RecordMessage>
    {
        private readonly KeeperDbContext dbContext;
        private readonly SignInManager signInManager;
        private readonly ResourceLoader resourceLoader;
        private RecordTree records;
        private bool ignoreParents;
        private int selectedSortOptionIndex;

        public HomeViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
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

        public List<SortingOption> SortingOptions => [
            new SortingOption(nameof(Record.Title), false, resourceLoader.GetString("AlphabeticalOrder")),
            new SortingOption(nameof(Record.Title), true, resourceLoader.GetString("AlphabeticalDescendingOrder")),
            new SortingOption(nameof(Record.Created), false, resourceLoader.GetString("CreatedOrder")),
            new SortingOption(nameof(Record.Created), true, resourceLoader.GetString("CreatedDescendingOrder")),
            new SortingOption(nameof(Record.Modified), false, resourceLoader.GetString("ModifiedOrder")),
            new SortingOption(nameof(Record.Modified), true, resourceLoader.GetString("ModifiedDescendingOrder"))
        ];

        public int SelectedSortOptionIndex 
        {
            get => selectedSortOptionIndex; 
            set => SetProperty(ref selectedSortOptionIndex, value); 
        }

        public RelayCommand<Record> DeleteRecordCommand => new(DeleteRecord);
        public RelayCommand<string> SearchCommand => new(Search);
        public RelayCommand<int> SortCommand => new(Sort);

        private void Sort(int obj)
        {
            var selectedOption = SortingOptions[obj];
            var prop = typeof(Record).GetProperty(selectedOption.PropertyName);
            IEnumerable<Record> recrods = selectedOption.IsDescending 
                ? Records.GetAllRecords().OrderByDescending(prop.GetValue) 
                : Records.GetAllRecords().OrderBy(prop.GetValue);
            Records = new RecordTree(recrods, ignoreParents);
        }

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

        public void Search(string query)
        {
            ignoreParents = !string.IsNullOrEmpty(query);
            var newTree = new RecordTree(dbContext.GetRecordsForUser(signInManager.CurrentUserName));
            Records = string.IsNullOrWhiteSpace(query) ? newTree : new RecordTree(newTree.SearchByTitle(query), true);
            Sort(SelectedSortOptionIndex);
        }
    }
}
