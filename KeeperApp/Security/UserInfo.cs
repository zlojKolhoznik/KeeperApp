using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Security
{
    /// <summary>
    /// Represents information about a user for purposes of security.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Hashed username that is stored in Credential Locker.
        /// </summary>
        [Key]
        public string UsernameHash { get; set; }

        [EncryptProperty]
        public string? Email { get; set; }

        public bool? IsEmailConfirmed { get; set; }

        /// <summary>
        /// The confirmation code that is used to verify email address or reset password.
        /// </summary>
        [EncryptProperty]
        public string? ConfirmationCode { get; set; }

        public DateTime? ConfirmationCodeExpiration { get; set; }
    }
}
