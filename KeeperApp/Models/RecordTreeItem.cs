using CommunityToolkit.Mvvm.ComponentModel;
using KeeperApp.Records;
using System.Collections.ObjectModel;

namespace KeeperApp.Models
{
    public class RecordTreeItem : ObservableObject
    {
        private Record record;

        public Record Record
        {
            get => record;
            set
            {
                record = value;
                OnPropertyChanged();
            }
        }

        public bool AllowDrop => Record is Folder;

        public ObservableCollection<RecordTreeItem> Children { get; set; }
    }
}
