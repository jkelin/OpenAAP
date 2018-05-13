using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class Session : ISession
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid IdentityId { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [JsonIgnore]
        public Identity Identity { get; set; }

        [NotMapped]
        public object Data { get; set; }
    }
}
