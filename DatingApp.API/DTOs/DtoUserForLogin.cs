using System.ComponentModel.DataAnnotations;

namespace DatingApp.DTOs
{
    public class DtoUserForLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}