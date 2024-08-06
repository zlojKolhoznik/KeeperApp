using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Views.ViewFactories
{
    public class FolderViewsFactory : IRecordViewsFactory
    {
        public ContentDialog GetAddDialog() => new AddFolderDialog();

        public Type GetDetailsPageType() => typeof(FolderDetailsPage);
    }
}
