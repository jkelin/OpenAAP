using System;
using System.ComponentModel.DataAnnotations;

namespace OpenAAP.Requests
{
    public class ValidatorRequest
    {
        [Required]
        [MinLength(5)]
        public string RequiredUsername { get; set; }

        [EmailAddress]
        public string OptionalEmail { get; set; }

        [Required]
        public Guid? RequiredId1 { get; set; }

        public Guid? OptionalId2 { get; set; }
    }
}
