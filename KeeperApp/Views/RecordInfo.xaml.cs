using KeeperApp.Records;
using KeeperApp.Records.ViewAttributes;
using KeeperApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecordInfo : Page
    {
        public static DependencyProperty RecordProperty = DependencyProperty.Register("Record", typeof(Record), typeof(RecordInfo), new PropertyMetadata(null));
        public static DependencyProperty IsInEditingModeProperty = DependencyProperty.Register("IsInEditingMode", typeof(bool), typeof(RecordInfo), new PropertyMetadata(false));

        public RecordInfo()
        {
            ViewModel = App.Current.Services.GetService<RecordInfoViewModel>();
            this.InitializeComponent();
        }

        public RecordInfoViewModel ViewModel { get; }

        public Record Record
        {
            get => (Record)GetValue(RecordProperty);
            set
            {
                SetValue(RecordProperty, value);
                ViewModel.Record = value;
            }
        }

        public bool IsInEditingMode
        {
            get => (bool)GetValue(IsInEditingModeProperty);
            set
            {
                SetValue(IsInEditingModeProperty, value);
                ViewModel.IsInEditingMode = value;
            }
        }
    }
}
