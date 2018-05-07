using System.ComponentModel.DataAnnotations;

namespace OpenAAP.Requests
{
    public class PasswordLoginRequest
    {
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
