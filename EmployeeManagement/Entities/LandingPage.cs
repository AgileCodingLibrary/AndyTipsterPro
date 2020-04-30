using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.Entities
{
    public class LandingPage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(12000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        public string LandingPageHtml { get; set; }

    }
}