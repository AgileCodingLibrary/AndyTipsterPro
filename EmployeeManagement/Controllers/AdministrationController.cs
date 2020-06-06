using AndyTipsterPro.Entities;
using AndyTipsterPro.Helpers;
using AndyTipsterPro.Models;
using AndyTipsterPro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AndyTipsterPro.Controllers
{


    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger,
                                        IConfiguration configuration,
                                        AppDbContext dbContext)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            _configuration = configuration;
            this._dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> ManageUserRoles()
        {

            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            ViewBag.userId = currentUser.Id;

            var user = await userManager.FindByIdAsync(currentUser.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(currentUser);
            var result = await userManager.RemoveFromRolesAsync(currentUser, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(currentUser,
        model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = currentUser.Id });
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    //throw new Exception("Test Exception");

                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role {ex}");

                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users " +
                        $"in this role. If you want to delete this role, please remove the users from " +
                        $"the role and then try to delete";
                    return View("Error");
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserProfile()
        {

            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }

            List<UserSubscriptions> userSubscriptions = _dbContext.UserSubscriptions.Where(x => x.UserId == user.Id).ToList();

            var model = new EditUserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                City = user.City,
                SendEmails = user.SendEmails,
                UserSubscriptions = userSubscriptions
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }
            else
            {
                List<UserSubscriptions> userSubscriptions = _dbContext.UserSubscriptions.Where(x => x.UserId == user.Id).ToList();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.City = model.City;
                user.SendEmails = model.SendEmails;
                user.Subscriptions = userSubscriptions;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("EditUserProfile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        [HttpGet]
        [Authorize(Roles = "superadmin, admin")]
        public ViewResult SearchUserByEmail()
        {
            //check if this is an admin user.
            if (!User.IsInRole("superadmin") || !User.IsInRole("admin"))
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> SubscribeUser(string Email)
        {

            var user = await userManager.FindByEmailAsync(Email);


            //check if this is an admin user.
            if (!User.IsInRole("superadmin") || !User.IsInRole("admin"))
            {
                RedirectToAction("Index", "Home");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }


            //protect Super admin

            if (user.Email.Contains("fazahmed786@hotmail.com"))
            {
                ViewBag.ErrorMessage = $"User cannot be found. --";
                return View("NotFound");
            }





            //build a view model to show all current manual subscriptions.
            var model = new SubscribeUserByAdminViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CanSeeComboPackage = user.CanSeeComboPackage,
                CanSeeElitePackage = user.CanSeeElitePackage,
                CanSeeUKRacingPackage = user.CanSeeUKRacingPackage,
                ManualComboPackageAccessExpiresAt = user.ManualComboPackageAccessExpiresAt,
                ManualElitePackageAccessExpiresAt = user.ManualElitePackageAccessExpiresAt,
                ManualUKRacingPackageAccessExpiresAt = user.ManualUKRacingPackageAccessExpiresAt
            };

            return View(model);

        }


        [HttpPost]
        [Authorize(Roles = "superadmin, admin")]
        public async Task<IActionResult> SaveUserSubscription(SubscribeUserByAdminViewModel model)
        {

            //save user subscription.

            var user = await userManager.FindByEmailAsync(model.Email);

            //check if this is an admin user.
            if (!User.IsInRole("superadmin") || !User.IsInRole("admin"))
            {
                RedirectToAction("Index", "Home");
            }


            if (user == null)
            {
                ViewBag.Message = $"User cannot be found, Nothing updated.";
                return View();
            }

            if (!user.EmailConfirmed)
            {
                ViewBag.Message = $"User have not confirmed their email. They should click on the confirmation link.";
                return View();
            }


            //update user and save.                

            user.CanSeeComboPackage = model.CanSeeComboPackage;
            user.CanSeeElitePackage = model.CanSeeElitePackage;
            user.CanSeeUKRacingPackage = model.CanSeeUKRacingPackage;
            user.ManualComboPackageAccessExpiresAt = model.ManualComboPackageAccessExpiresAt;
            user.ManualElitePackageAccessExpiresAt = model.ManualElitePackageAccessExpiresAt;
            user.ManualUKRacingPackageAccessExpiresAt = model.ManualUKRacingPackageAccessExpiresAt;

            await userManager.UpdateAsync(user);

            ViewBag.Message = $"WELL DONE! You successfully updated user subscriptions. Ask your customer to login and enjoy their updates.";




            return View();

        }


        [HttpGet]
        [Authorize(Roles = "superadmin, admin")]
        public async Task<IActionResult> UserDashboard(string filter, int page = 1, int pageSize = 5)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }

            //Pagination NuGet.

            IOrderedQueryable<ApplicationUser> query = null;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = userManager.Users.Where(x => x.Email.Contains(filter) ||
                                                x.FirstName.Contains(filter) ||
                                                x.LastName.Contains(filter) ||
                                                x.UserName.Contains(filter)).AsNoTracking().OrderBy(x => x.Email);
            }

            query = userManager.Users.AsNoTracking().OrderBy(x => x.Email);

            var model = await PagingList.CreateAsync(query, pageSize, page);
            model.Action = "UserDashboard";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "superadmin, admin")]
        public async Task<IActionResult> UserDetails(string userId)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (!User.IsInRole("admin") || !User.IsInRole("superadmin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new UserDetailsViewModel
            {
                Customer = userManager.Users.Where(x => x.Id == userId).FirstOrDefault(),

                CustomerSubscriptions = _dbContext.UserSubscriptions.Where(x => x.UserId == userId).ToList()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "superadmin, admin")]
        public ViewResult BroadCastMessage()
        {
            //check if this is an admin user.
            if (!User.IsInRole("superadmin") || !User.IsInRole("admin"))
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "superadmin, admin")]
        public ViewResult BroadCastMessage(string message)
        {
            //check if this is an admin user.
            if (!User.IsInRole("superadmin") || !User.IsInRole("admin"))
            {
                RedirectToAction("Index", "Home");
            }


            //List<string> emails = userManager.Users.Select(x => x.Email).ToList();

            List<string> emails = new List<string>();
            emails.Add("fazahmed786@hotmail.com");
            emails.Add("fazahmed20xx@hotmail.com");

             var sendGridKey = _configuration.GetValue<string>("SendGridApi");

            foreach (var email in emails)
            {
                Task.Run(() => Emailer.SendEmail(email, "An Important Announcement has been made by Andy Tipster.", message, sendGridKey).Wait());
            }

            return View("MessageBroadCasted");
        }
    }
}
