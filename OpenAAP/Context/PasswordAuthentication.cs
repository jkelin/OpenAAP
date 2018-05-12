using OpenAAP.Services.PasswordHashing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    /// <summary>
    /// Represents user's login with password
    /// After password change new row is to be added instead of modification of existing row
    /// </summary>
    public class PasswordAuthentication
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid IdentityId { get; set; }

        public Identity Identity { get; set; }

        [Required]
        public byte[] Hash { get; set; }

        [Required]
        public byte[] Salt { get; set; }

        [Required]
        public PasswordAuthenticationHashAlgorithm Algorithm { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// This password login has been disabled (presumably because new password has been configured)
        /// These password records will be removed after configured period of time
        /// </summary>
        public DateTime? DisabledAt { get; set; }
    }
}
