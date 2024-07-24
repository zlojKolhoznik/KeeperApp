using KeeperApp.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace KeeperApp.Behaviors
{
    public class ValidateInputBehavior : Behavior<ContentDialog>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += AssociatedObject_Closing;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closing -= AssociatedObject_Closing;
        }

        private void AssociatedObject_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            bool validationNeeded = args.Result == ContentDialogResult.Primary;
            IValidator validator = (IValidator)AssociatedObject.DataContext;
            if (validationNeeded && !validator.IsInputValid())
            {
                args.Cancel = true;
            }
        }
    }
}
