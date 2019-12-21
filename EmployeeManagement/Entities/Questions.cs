using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.Entities
{
    public class Questions
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        public string Question { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Answer { get; set; }
    }
}