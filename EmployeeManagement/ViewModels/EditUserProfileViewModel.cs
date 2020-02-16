using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.ViewModels
{
    public class EditUserProfileViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        public bool SendEmails { get; set; }
    }


}