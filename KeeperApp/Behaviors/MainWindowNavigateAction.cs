using KeeperApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;

namespace KeeperApp.Behaviors
{
    public class MainWindowNavigateAction : DependencyObject, IAction
    {
        public static DependencyProperty FrameProperty = DependencyProperty.Register("Frame",
            typeof(Frame), 
            typeof(MainWindowNavigateAction),
            new PropertyMetadata(null));

        public Frame Frame
        {
            get => (Frame)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            NavigationViewItemInvokedEventArgs args = (NavigationViewItemInvokedEventArgs)parameter;
            var result = false;
            if (args.IsSettingsInvoked)
            {
                Frame.Navigate(typeof(SettingsPage));
            }
            else if (args.InvokedItemContainer != null && args.InvokedItemContainer.Tag != null)
            {
                Type pageToNavigate = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                result = Frame.Navigate(pageToNavigate);
            }
            else if (args.InvokedItemContainer.Content != null && args.InvokedItemContainer.Content.ToString() == "Exit")
            {
                Application.Current.Exit();
            }
            return result;
        }
    }
}
