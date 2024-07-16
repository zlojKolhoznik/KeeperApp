using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;

namespace KeeperApp.Behaviors
{
    public class InvokeContentDialogAction : DependencyObject, IAction
    {
        public static DependencyProperty ContentDialogProperty = DependencyProperty.RegisterAttached("ContentDialog", 
            typeof(Type),
            typeof(InvokeContentDialogAction),
            new PropertyMetadata(null));

        public static DependencyProperty ArgumentProperty = DependencyProperty.RegisterAttached("Argument",
            typeof(object),
            typeof(InvokeContentDialogAction),
            new PropertyMetadata(null));

        public Type ContentDialog 
        {
            get => (Type)GetValue(ContentDialogProperty);
            set
            {
                if (!typeof(ContentDialog).IsAssignableFrom(value))
                {
                    throw new ArgumentException("ContentDialogName must be a type of ContentDialog");
                }
                SetValue(ContentDialogProperty, value);
            }
        }

        public object Argument
        {
            get => GetValue(ArgumentProperty);
            set => SetValue(ArgumentProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            var dialog = (ContentDialog)(Argument is null ? Activator.CreateInstance(ContentDialog) : Activator.CreateInstance(ContentDialog, Argument));
            dialog.XamlRoot = (sender as FrameworkElement).XamlRoot;
            dialog.ShowAsync();
            return true;
        }
    }
}
