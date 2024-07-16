using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Messaging
{
    public class RecordMessage
    {
        public Record Record { get; set; }
        public RecordMessageType MesasgeType { get; set; }
    }

    public enum RecordMessageType
    {
        Added,
        Updated,
        Deleted
    }
}
