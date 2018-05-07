using System.ComponentModel.DataAnnotations;

namespace OpenAAP.Requests
{
    public class PasswordRegisterRequest
    {
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
