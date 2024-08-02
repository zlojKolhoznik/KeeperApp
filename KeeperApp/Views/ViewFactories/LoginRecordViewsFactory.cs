using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Views.ViewFactories
{
    public class LoginRecordViewsFactory : IRecordViewsFactory
    {
        public Type GetDetailsPageType() => typeof(LoginRecordDetailsPage);

        public ContentDialog GetAddDialog() => new AddLoginDialog();
    }
}
