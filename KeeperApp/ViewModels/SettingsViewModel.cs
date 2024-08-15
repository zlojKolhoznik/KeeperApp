using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Database;
using KeeperApp.Records;
using KeeperApp.Security;
using KeeperApp.Security.Authentication;
using KeeperApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace KeeperApp.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;
        private readonly KeeperDbContext dbContext;
        private readonly RecordsSerializationService serializationService;
        private readonly ResourceLoader resourceLoader;
        private bool isWindowsHelloAvailable;
        private string currentPassword;
        private string newPassword;
        private string confirmPassword;
        private string deleteAccountPassword;
        private string email;
        private string emailConfirmation;
        private bool isWindowsHelloConnected;
        private string changePasswordError;
        private string emailConfirmationError;
        private bool isEmailConfirmationActive;

        public SettingsViewModel(SignInManager signInManager, KeeperDbContext dbContext, IEmailSender sender, RecordsSerializationService serializationService)
        {
            this.signInManager = signInManager;
            this.dbContext = dbContext;
            this.serializationService = serializationService;
            resourceLoader = new ResourceLoader();
            var userInfo = dbContext.Users.Find(Sha256Hasher.GetHash(signInManager.CurrentUserName));
            Email = userInfo?.Email;
            IsEmailConfirmationActive = userInfo?.ConfirmationCode is not null && userInfo.ConfirmationCodeExpiration > DateTime.Now && !(userInfo.IsEmailConfirmed ?? false);
            CheckWindowsHello();
        }

        public string Title => resourceLoader.GetString("Settings");
        public string WindwsHelloDescription => resourceLoader.GetString("WindwsHelloDescription");
        public string EnableWindowsHelloLabel => resourceLoader.GetString("EnableWindowsHello");
        public string DisableWindowsHelloLabel => resourceLoader.GetString("DisableWindowsHello");
        public string WindowsHelloUnaccessible => resourceLoader.GetString("WindowsHelloUnaccessible");
        public string ChangeMasterPassword => resourceLoader.GetString("ChangeMasterPassword");
        public string CurrentPasswordLabel => resourceLoader.GetString("CurrentPassword");
        public string NewPasswordLabel => resourceLoader.GetString("NewPassword");
        public string ConfirmNewPasswordLabel => resourceLoader.GetString("ConfirmNewPassword");
        public string DeleteAccountLabel => resourceLoader.GetString("DeleteAccount");
        public string DeleteAccountWarning => resourceLoader.GetString("DeleteAccountWarning");
        public string DeleteAccountPrompt => resourceLoader.GetString("DeleteAccountPrompt");
        public string Language => resourceLoader.GetString("Language");
        public string Delete => resourceLoader.GetString("Delete");
        public string Cancel => resourceLoader.GetString("Cancel");
        public string Save => resourceLoader.GetString("Save");
        public string Confirm => resourceLoader.GetString("Confirm");
        public string ConfirmationCodePlaceholder => resourceLoader.GetString("ConfirmationCode");
        public string EmailConfirmationDescription => resourceLoader.GetString("EmailConfirmationDescription");
        public string ExportAndImportHeader => resourceLoader.GetString("ExportAndImport");
        public string ExportDescription => resourceLoader.GetString("ExportDescription");
        public string ImportDescription => resourceLoader.GetString("ImportDescription");

        public bool IsWindowsHelloConnected
        {
            get => isWindowsHelloConnected;
            set => SetProperty(ref isWindowsHelloConnected, value);
        }

        public bool IsWindowsHelloAvailable 
        {
            get => isWindowsHelloAvailable;
            set => SetProperty(ref isWindowsHelloAvailable, value);
        }

        public string DeleteAccountPassword
        {
            get => deleteAccountPassword;
            set => SetProperty(ref deleteAccountPassword, value);
        }

        public string CurrentPassword 
        {
            get => currentPassword; 
            set => SetProperty(ref currentPassword, value); 
        }

        public string NewPassword 
        { 
            get => newPassword; 
            set => SetProperty(ref newPassword, value);
        }

        public string ConfirmPassword 
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string EmailConfirmation
        {
            get => emailConfirmation;
            set => SetProperty(ref emailConfirmation, value);
        }

        public string EmailConfirmationError
        {
            get => emailConfirmationError;
            set => SetProperty(ref emailConfirmationError, value);
        }

        public bool IsEmailConfirmationActive
        {
            get => isEmailConfirmationActive;
            set => SetProperty(ref isEmailConfirmationActive, value);
        }

        public string ChangePasswordError
        {
            get => changePasswordError;
            set => SetProperty(ref changePasswordError, value);
        }

        public bool IsEmailConfirmed => signInManager.IsEmailConfirmed;

        public RelayCommand ChangePasswordCommand => new(ChangePassword);
        public AsyncRelayCommand DeleteAccountCommand => new(DeleteAccount);
        public AsyncRelayCommand EnableWindowsHelloCommand => new(EnableWindowsHello);
        public AsyncRelayCommand DisableWindowsHelloCommand => new(DisableWindowsHello);
        public AsyncRelayCommand SaveEmailCommand => new(SaveEmail);
        public AsyncRelayCommand ConfirmEmailCommand => new(ConfirmEmail);
        public AsyncRelayCommand ExportRecordsCommand => new(ExportRecords);
        public AsyncRelayCommand ImportRecordsCommand => new(ImportRecords);

        private async Task ExportRecords()
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "records",
                DefaultFileExtension = ".json"
            };
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.Current.MainWindow));
            picker.FileTypeChoices.Add("JSON", [".json"]);
            var file = await picker.PickSaveFileAsync();
            if (file is not null)
            {
                var records = dbContext.GetRecordsForUser(signInManager.CurrentUserName).Where(r => r.GetType() != typeof(Folder));
                await serializationService.SerializeRecordsAsync(records, file);
            }
        }

        private async Task ImportRecords()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.Current.MainWindow));
            picker.FileTypeFilter.Add(".json");
            var file = await picker.PickSingleFileAsync();
            if (file is not null)
            {
                var records = await serializationService.DeserializeRecordsAsync(file);
                foreach (var record in records)
                {
                    AddRecord(record);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        private void AddRecord(Record record)
        {
            record.Created = DateTime.Now;
            record.Modified = DateTime.Now;
            record.Id = 0;
            record.OwnerUsernameHash = Sha256Hasher.GetSaltedHash(signInManager.CurrentUserName, record.Created.ToString());
            if (record.Parent is not null)
            {
                AddRecord(record.Parent);
                record.ParentId = null;
            }
            dbContext.Add(record);
        }

        private async Task ConfirmEmail()
        {
            try
            {
                await signInManager.ConfirmEmailAsync(EmailConfirmation);
                IsEmailConfirmationActive = false;
                OnPropertyChanged(nameof(IsEmailConfirmed));
            }
            catch (InvalidOperationException)
            {
                EmailConfirmation = "";
                EmailConfirmationError = "Invalid confirmation code. Try again";
            }
        }

        private async Task SaveEmail()
        {
            await signInManager.SetEmailAsync(Email);
            IsEmailConfirmationActive = true;
            OnPropertyChanged(nameof(IsEmailConfirmed));
        }

        private async Task EnableWindowsHello()
        {
            IsWindowsHelloConnected = await signInManager.ConnectWindowsHelloAsync();
        }

        private async Task DisableWindowsHello()
        {
            IsWindowsHelloConnected = !await signInManager.DisconnectWindowsHello();
        }

        private void ChangePassword()
        {
            if (NewPassword == ConfirmPassword)
            {
                bool changed = signInManager.ChangePassword(signInManager.CurrentUserName, CurrentPassword, NewPassword);
                if (changed)
                {
                    CurrentPassword = "";
                    NewPassword = "";
                    ConfirmPassword = "";
                    ChangePasswordError = resourceLoader.GetString("PasswordChangedSuccessfully");
                }
                else
                {
                    ChangePasswordError = resourceLoader.GetString("IncorrectPassword");
                }
            }
            else
            {
                ChangePasswordError = resourceLoader.GetString("PasswordMismatch");
            }
        }

        private async Task DeleteAccount()
        {
            if (signInManager.Unregister(signInManager.CurrentUserName, DeleteAccountPassword))
            {
                if (await signInManager.IsWindowsHelloConnectedAsync())
                {
                    await signInManager.DisconnectWindowsHello(true);
                }
                var recordsToDelete = dbContext.GetRecordsForUser(signInManager.CurrentUserName);
                foreach (var record in recordsToDelete)
                {
                    dbContext.Remove(record);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private async void CheckWindowsHello()
        {
            IsWindowsHelloAvailable = await WindowsHelloHelper.IsWindowsHelloAvailableAsync();
            IsWindowsHelloConnected = await signInManager.IsWindowsHelloConnectedAsync();
        }
    }
}
