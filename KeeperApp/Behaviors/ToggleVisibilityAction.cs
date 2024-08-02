using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace KeeperApp.Behaviors
{
    internal class ToggleVisibilityAction : DependencyObject, IAction
    {
        public static DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(UIElement), typeof(ToggleVisibilityAction), new PropertyMetadata(null));
        public static DependencyProperty InvokerProperty = DependencyProperty.Register("Invoker", typeof(ContentControl), typeof(ToggleVisibilityAction), new PropertyMetadata(null));

        public UIElement Target
        {
            get => (UIElement)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public ContentControl Invoker
        {
            get => (ContentControl)GetValue(InvokerProperty);
            set => SetValue(InvokerProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            Target.Visibility = Target.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            Invoker.Content = Target.Visibility == Visibility.Visible ? "–" : "+";
            return null;
        }
    }
}
