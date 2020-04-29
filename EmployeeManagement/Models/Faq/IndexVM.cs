using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AndyTipsterPro.Entities;

namespace AndyTipsterPro.Models.Faq
{
    public class IndexVM
    {
        public List<Testimonial> Questions { get; set; } = new List<Testimonial>();
    }
}