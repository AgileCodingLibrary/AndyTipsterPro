using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.Entities
{
    public class Tips
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Elite Package Tips")]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        public string ElitePackageTips { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DisplayName("Combination Package Tips")]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        public string CombinationPackageTips { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DisplayName("UK Package Tips")]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        public string UKPackageTips { get; set; }
    }


}