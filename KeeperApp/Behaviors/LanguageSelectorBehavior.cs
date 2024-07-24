using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Linq;

namespace KeeperApp.Behaviors
{
    public class LanguageSelectorBehavior : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItem = AssociatedObject.Items.Cast<ComboBoxItem>().FirstOrDefault(predicate: i => i.Tag.ToString() == App.Current.Language);
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            App.Current.LanguageChanged += OnLanguageChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
            App.Current.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            var frame = GetParentFrame(AssociatedObject);
            frame?.Navigate(frame.Content.GetType());
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItem is ComboBoxItem selectedItem)
            {
                App.Current.Language = selectedItem.Tag.ToString();
            }
        }

        private Frame GetParentFrame(FrameworkElement element)
        {
            if (element is Frame frame)
            {
                return frame;
            }
            else if (element is FrameworkElement frameworkElement)
            {
                return GetParentFrame(frameworkElement.Parent as FrameworkElement);
            }
            return null;
        }
    }
}
