using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.Behaviors
{
    public class MainWindowLanguageBehavior : Behavior
    {
        private readonly ResourceLoader resourceLoader;

        public static DependencyProperty NavbarProperty = DependencyProperty.Register("Navbar", typeof(NavigationView), typeof(MainWindowLanguageBehavior), new PropertyMetadata(null));

        public MainWindowLanguageBehavior()
        {
            resourceLoader = new ResourceLoader();
        }

        public NavigationView Navbar
        {
            get => (NavigationView)GetValue(NavbarProperty);
            set => SetValue(NavbarProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            App.Current.LanguageChanged += OnLanguageChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            App.Current.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            foreach (var item in Navbar.MenuItems.Append(Navbar.PaneFooter))
            {
                if (item is NavigationViewItem navigationViewItem)
                {
                    navigationViewItem.Content = resourceLoader.GetString(navigationViewItem.Name);
                }
            }
            ((NavigationViewItem)Navbar.SettingsItem).Content = resourceLoader.GetString("Settings");
        }
    }
}
