using System.ComponentModel.DataAnnotations;

namespace Noteapp.Web.ViewModels
{
    public class RegisterViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
