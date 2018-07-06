using System.ComponentModel.DataAnnotations;

namespace SN_App.Api.Dtos
{
    public class UserRegistrationDto
    {
        [Required]
        public string Username { get; set; }
        [Required]        
        [StringLength(10, MinimumLength = 4, ErrorMessage = "You must provide between 4 and 10 chars!" )]
        public string Password { get; set; }
    }
}