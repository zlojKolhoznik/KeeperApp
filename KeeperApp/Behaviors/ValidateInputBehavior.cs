using KeeperApp.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (args.Result == ContentDialogResult.Primary && !((IValidator)AssociatedObject.DataContext).IsInputValid())
            {
                args.Cancel = true;
                sender.ContextFlyout.ShowAt(sender);
            }
        }
    }
}
