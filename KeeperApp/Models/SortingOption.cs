using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Models
{
    public readonly struct SortingOption
    {
        public SortingOption(string propertyName, bool isDescending, string displayName)
        {
            PropertyName = propertyName;
            IsDescending = isDescending;
            DisplayName = displayName;
        }

        public string PropertyName { get; }
        public bool IsDescending { get; }
        public string DisplayName { get; }
    }
}
