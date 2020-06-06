using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.ViewModels
{
    public class BroadcastMessageViewModel
    {
        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        public string Subject { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}