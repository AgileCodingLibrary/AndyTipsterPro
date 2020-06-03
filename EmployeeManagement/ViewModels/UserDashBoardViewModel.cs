using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using System.Collections.Generic;

namespace AndyTipsterPro.ViewModels
{
    public class UserDashBoardViewModel
    {
        public ApplicationUser currentUser { get; set; }
        public List<ApplicationUser> Customers { get; set; } = new List<ApplicationUser>();
        public List<UserSubscriptions> CustomerSubscriptions { get; set; } = new List<UserSubscriptions>();
    }
}
