using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class Identity
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

        [JsonIgnore]
        public ICollection<PasswordAuthentication> PasswordAuthentications { get; set; } = new List<PasswordAuthentication>();
    }
}
