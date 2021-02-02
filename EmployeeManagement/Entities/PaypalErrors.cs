using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndyTipsterPro.Entities
{
    public class PaypalErrors
    {
        public int Id { get; set; }
        public string Exception { get; set; }
        public DateTime DateTime { get; set; }
    }
}
