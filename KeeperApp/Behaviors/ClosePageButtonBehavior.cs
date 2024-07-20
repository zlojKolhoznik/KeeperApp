using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Behaviors
{
    internal class ClosePageButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var parent = AssociatedObject.Parent;
            while (parent is not null and not Page)
            {
                parent = ((FrameworkElement)parent).Parent;
            }
            if (parent is Page page)
            {
                ((Panel)page.Parent).Children.Clear();
            }
        }
    }
}
