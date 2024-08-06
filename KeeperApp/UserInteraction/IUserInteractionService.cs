using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.UserInteraction
{
    public interface IUserInteractionService
    {
        Task<bool> AskUserYesNo(string message);
    }
}
