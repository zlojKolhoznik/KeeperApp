using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Views.ViewFactories
{
    internal class CardCredentialsRecordViewsFactory : IRecordViewsFactory
    {
        public ContentDialog GetAddDialog() => new AddCardCredentialsDialog();

        public Type GetDetailsPageType() => typeof(CardCredentialsRecordDetailsPage);
    }
}
