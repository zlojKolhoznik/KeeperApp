using KeeperApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace KeeperApp.Behaviors
{
    public class MainTreeDragAndDropBehavior : Behavior<TreeViewItem>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.AllowDrop = ((RecordTreeItem)AssociatedObject.DataContext).AllowDrop;
        }
    }
}
