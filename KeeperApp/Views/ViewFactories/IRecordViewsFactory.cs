using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Views.ViewFactories
{
    public interface IRecordViewsFactory
    {
        /// <returns>Type of the page that can then be used to navigate within the frame</returns>
        Type GetDetailsPageType();
        ContentDialog GetAddDialog();
    }
}
