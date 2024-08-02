using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using KeeperApp.Security;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public abstract class AddRecordViewModel : ObservableObject
    {
        private readonly KeeperDbContext dbContext;
        private readonly SignInManager signInManager;
        private string errorMessage;
        private Record record;
        private string recordTitle;
        protected readonly ResourceLoader resourceLoader;

        public AddRecordViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            resourceLoader = new ResourceLoader();
        }

        public AsyncRelayCommand SaveRecordCommand => new(HandleSaveRecordCommandAsync);

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public Record Record
        {
            get => record;
            set => SetProperty(ref record, value);
        }

        public string RecordTitle
        {
            get => recordTitle;
            set => SetProperty(ref recordTitle, value);
        }

        public string PageTitle => $"{resourceLoader.GetString("AddRecordTitle")}";
        public string Save => resourceLoader.GetString("Save");
        public string Cancel => resourceLoader.GetString("Cancel");
        public string TitlePlaceholder => resourceLoader.GetString("Title");

        private async Task HandleSaveRecordCommandAsync()
        {
            if (IsInputValid())
            {
                await SaveRecordAsync();
            }
            else
            {
                ErrorMessage = resourceLoader.GetString("RequiredFieldsPrompt");
            }
        }

        protected virtual async Task SaveRecordAsync()
        {
            Record.Title = RecordTitle;
            Record.IsDeleted = false;
            Record.Created = DateTime.Now;
            Record.Modified = new DateTime(1970, 1, 1);
            Record.OwnerUsernameHash = Sha256Hasher.GetSaltedHash(signInManager.CurrentUserName, Record.Created.ToString());
            dbContext.Add(Record);
            await dbContext.SaveChangesAsync();
            WeakReferenceMessenger.Default.Send(new RecordMessage { MesasgeType = RecordMessageType.Added, Record = Record });
        }

        public abstract bool IsInputValid();
    }
}
