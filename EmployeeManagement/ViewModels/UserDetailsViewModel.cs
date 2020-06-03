using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using System.Collections.Generic;

namespace AndyTipsterPro.ViewModels
{
    public class UserDetailsViewModel
    {
        public ApplicationUser Customer { get; set; }
        public List<UserSubscriptions> CustomerSubscriptions { get; set; }
    }
}
