using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AndyTipsterPro.Entities
{
    public class About
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DisplayName("Page heading")]
        public string PageHeader { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("First top paragraph")]
        public string FirstTopParagraph { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("Second top paragraph")]
        public string SecondTopParagraph { get; set; }

        [DisplayName("A title to describe your packages")]
        public string PackagesTitle { get; set; }

        [DisplayName("Package one heading")]
        public string PackageOneHeading { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("Package one details")]
        public string PackageOneDetails { get; set; }

        [DisplayName("Package two heading")]
        public string PackageTwoHeading { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("Package two details")]
        public string PackageTWoDetails { get; set; }

        [DisplayName("Package three heading")]
        public string PackageThreeHeading { get; set; }
        
        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("Package three details")]
        public string PackageThreeDetails { get; set; }


    }
}