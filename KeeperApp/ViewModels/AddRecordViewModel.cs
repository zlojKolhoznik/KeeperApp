using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Messaging;
using KeeperApp.Records;
using KeeperApp.Records.ViewAttributes;
using KeeperApp.Security;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class AddRecordViewModel : ObservableObject, IValidator
    {
        private readonly KeeperDbContext dbContext;
        private readonly SignInManager signInManager;
        private readonly ResourceLoader resourceLoader;

        private Dictionary<string, string> properties;
        private Type recordType;
        private string errorMessage;
        private IEnumerable<ComboBoxItem> recordTypes;

        public AddRecordViewModel(KeeperDbContext dbContext, SignInManager signInManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            resourceLoader = new ResourceLoader();
            Properties = new Dictionary<string, string>();
            RecordType = (Type)RecordTypes.First().Tag;
        }

        public AsyncRelayCommand SaveRecordCommand => new(HandleSaveRecordCommandAsync);
        public RelayCommand<ComboBoxItem> RecordTypeChangedCommand => new((item) => RecordType = (Type)item.Tag);

        public Dictionary<string, string> Properties
        {
            get => properties;
            set => SetProperty(ref properties, value);
        }

        public Type RecordType
        {
            get => recordType;
            set
            {
                if (!value.IsSubclassOf(typeof(Record)) || value.IsAbstract)
                {
                    throw new ArgumentException("Type must be a conrete subclass of Record");
                }
                if (SetProperty(ref recordType, value))
                {
                    Properties.Clear();
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public IEnumerable<ComboBoxItem> RecordTypes => recordTypes ??= Assembly.GetAssembly(typeof(Record)).GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(Record)) && !t.IsAbstract)
                        .Select(t => new ComboBoxItem { Content = resourceLoader.GetString(t.Name), Tag = t });

        public string Title => $"{resourceLoader.GetString("AddRecordTitle")} {resourceLoader.GetString(RecordType.Name)}";
        public string Save => resourceLoader.GetString("Save");
        public string Cancel => resourceLoader.GetString("Cancel");

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

        private async Task SaveRecordAsync()
        {
            Record record = (Record)Activator.CreateInstance(RecordType);
            foreach (var prop in RecordType.GetProperties())
            {
                if (Properties.TryGetValue(prop.Name, out string value))
                {
                    prop.SetValue(record, value);
                }
            }
            record.IsDeleted = false;
            record.Created = DateTime.Now;
            record.Modified = new DateTime(1970, 1, 1);
            record.OwnerUsernameHash = Sha256Hasher.GetSaltedHash(signInManager.CurrentUserName, record.Created.ToString());
            dbContext.Add(record);
            await dbContext.SaveChangesAsync();
            WeakReferenceMessenger.Default.Send(new RecordMessage { MesasgeType = RecordMessageType.Added, Record = record });
        }

        public bool IsInputValid()
        {
            bool result = true;
            foreach (var prop in RecordType.GetProperties())
            {
                if (prop.GetCustomAttribute<RequiredPropertyAttribute>() is not null)
                {
                    if (!Properties.TryGetValue(prop.Name, out string value) || string.IsNullOrWhiteSpace(value))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
