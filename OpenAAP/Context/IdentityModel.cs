using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    [Table("identity")]
    public class IdentityModel
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [EmailAddress]
        [MaxLength(64)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string UserName { get; set; }

        public List<PasswordAuthenticationModel> PasswordAuthentication { get; set; }
    }
}
