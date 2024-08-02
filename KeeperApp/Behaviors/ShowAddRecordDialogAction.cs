using KeeperApp.Views.ViewFactories;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Behaviors
{
    public class ShowAddRecordDialogAction : DependencyObject, IAction
    {
        public static DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(IRecordViewsFactory), typeof(ShowAddRecordDialogAction), new PropertyMetadata(null));

        public IRecordViewsFactory Factory
        {
            get => (IRecordViewsFactory)GetValue(FactoryProperty);
            set => SetValue(FactoryProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            var dialog = Factory.GetAddDialog();
            dialog.XamlRoot = (sender as FrameworkElement).XamlRoot;
            dialog.ShowAsync();
            return null;
        }
    }
}
