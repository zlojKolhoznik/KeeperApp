using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Database;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class RecordInfoViewModel : ObservableObject
    {
        private Record record;
        private bool isInEditingMode;
        private readonly KeeperDbContext dbContext;

        public RecordInfoViewModel(KeeperDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Record Record
        {
            get => record;
            set => SetProperty(ref record, value, nameof(Record));
        }

        public bool IsInEditingMode
        {
            get => isInEditingMode;
            set => SetProperty(ref isInEditingMode, value, nameof(IsInEditingMode));
        }

        public RelayCommand EnterEditingModeCommand => new(() => IsInEditingMode = true);

        public RelayCommand SaveChangesCommand => new(() =>
        {
            dbContext.Update(Record);
            dbContext.SaveChanges();
            IsInEditingMode = false;
        });

        public RelayCommand CancelChangesCommand => new(() =>
        {
            var values = dbContext.Entry(Record).GetDatabaseValues();
            foreach (var property in values.Properties)
            {
                Record.GetType().GetProperty(property.Name).SetValue(Record, values[property]);
            }
            IsInEditingMode = false;
        });

    }
}
