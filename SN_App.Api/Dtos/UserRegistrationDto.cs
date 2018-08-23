using System;
using System.ComponentModel.DataAnnotations;

namespace SN_App.Api.Dtos
{
    public class UserRegistrationDto
    {
        [Required]
        public string Username { get; set; }
        [Required]        
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must provide between 4 and 8 chars!" )]
        public string Password { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserRegistrationDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}