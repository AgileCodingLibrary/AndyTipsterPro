using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AndyTipsterPro.Models.Subscription
{
    public class IndexVm
    {
        public List<Plan> Plans { get; set; }

        public IndexVm()
        {
            Plans = new List<Plan>();
        }
    }
}