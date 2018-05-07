using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class SessionModel
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public Guid IdentityId { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
