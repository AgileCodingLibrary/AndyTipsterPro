using AndyTipsterPro.Entities;
using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AndyTipsterPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool SendEmails { get; set; }
        public List<UserSubscriptions> Subscriptions { get; set; } = new List<UserSubscriptions>();

    }

}