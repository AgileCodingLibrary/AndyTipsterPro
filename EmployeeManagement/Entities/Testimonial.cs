using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.Entities
{
    public class Testimonial
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]              
        public string Customer { get; set; }
    }
}