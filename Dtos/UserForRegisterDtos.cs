using System.ComponentModel.DataAnnotations;

namespace myDotnetApp.API.Dtos
{
    public class UserForRegisterDtos
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(16, MinimumLength=8, ErrorMessage="Length Error")]
        public string Password { get; set; }
    }
}