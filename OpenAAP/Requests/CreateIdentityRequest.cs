using System.ComponentModel.DataAnnotations;

namespace OpenAAP.Requests
{
    public class CreateIdentityRequest
    {
        [MaxLength(255)]
        public string Description { get; set; }

        [EmailAddress]
        [MaxLength(64)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string UserName { get; set; }
    }
}
