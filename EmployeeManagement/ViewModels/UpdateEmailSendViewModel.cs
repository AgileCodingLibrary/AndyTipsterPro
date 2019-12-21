using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FazTipster.ViewModels
{
    public class UpdateEmailSendViewModel
    {
        [Required]
        public bool SendEmails { get; set; }
    }
}