using System.ComponentModel.DataAnnotations;

namespace SN_App.Api.Dtos
{
    public class UserLoginDto
    {
        [Required] 
        public string Username { get; set; }
        public string Password { get; set; }
    }
}