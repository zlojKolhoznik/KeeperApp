using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Models
{
    public class PasswordAnalysisResult
    {
        public Record Record { get; set; }
        public bool Pass { get; set; }
        public int Score { get; set; }
        public IEnumerable<string> FailureMessages { get; set; }
    }
}
