using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Areas.Identity.Pages.Account;
using ObserveAssign.Data;
using ObserveAssign.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;

namespace ObserveAssign.Pages.User
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ObserveAssignDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public CreateModel(ObserveAssignDbContext context,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<IdentityUser>)_userStore;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public IActionResult OnGet()
        {
            SetUserRoles();
            return Page();
        }

        private void SetUserRoles()
        {
            var userRoles = _roleManager.Roles.ToList();

            UserRoles = new SelectList(userRoles, "Id", "Name");

            if (CurrentUserRole == null)
            {
                CurrentUserRole = new UserRoleModel();
            }

            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.ToList();

                ProjectList = new SelectList(projects, "ID", "Name");
            }
            if (UserModel == null)
            {
                UserModel = new AspNetUserModel();
            }
        }

        [BindProperty]
        public AspNetUserModel UserModel { get; set; }

        public SelectList UserRoles { get; set; }
        public SelectList ProjectList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Role is required")]
        [BindProperty]
        public UserRoleModel CurrentUserRole { get; set; }

        [BindProperty]
        [Display(Name = "Project")]
        public List<int> SelectedProjects { get; set; }
        public List<string> errorMessages { get; set; } = new List<string>();

        private List<string> validateGetErrors()
        {
            if (CurrentUserRole == null || CurrentUserRole.RoleId == "-1")
            {
                errorMessages.Add("Role is required");
            }
            if (string.IsNullOrEmpty(UserModel.FirstName))
            {
                errorMessages.Add("First Name is required");
            }
            if (string.IsNullOrEmpty(UserModel.FirstName))
            {
                errorMessages.Add("Last Name is required");
            }

            return errorMessages;
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //check for errors on nullable fields
            errorMessages = validateGetErrors();

            if (errorMessages.Count > 0)
            {
                SetUserRoles();
                //return user back to page
                return Page();
            }

            //set username as email address
            UserModel.UserName = UserModel.Email.Trim();

            if (UserModel.UserRoles == null)
            {
                UserModel.UserRoles = new List<UserRoleModel>();
                UserModel.UserRoles.Add(CurrentUserRole);

                ModelState.ClearValidationState(nameof(UserModel));
            }

            if (!TryValidateModel(UserModel))
            {
                //Create new user - add to database - set random password to start, user must reset
                var user = Activator.CreateInstance<IdentityUser>();
                user.LockoutEnabled = false;
                user.EmailConfirmed = true;
                user.NormalizedEmail = UserModel.Email.ToUpper();
                user.NormalizedUserName = UserModel.Email.ToUpper();

                await _userStore.SetUserNameAsync(user, UserModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, UserModel.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user);

                string projectsAssigned = string.Empty;
                string projectAdminName = string.Empty;
                string projectAdminEmail = string.Empty;

                IdentityUser updatedUser = _userManager.Users.Where(u => u.Email == UserModel.Email).FirstOrDefault();
                if (updatedUser != null)
                {
                    //assign to user roles
                    if (UserRoles == null)
                    {
                        var userRoles = _roleManager.Roles.ToList();

                        UserRoles = new SelectList(userRoles, "Id", "Name");
                    }
                    string newRoleName = UserRoles.Where(u => u.Value == CurrentUserRole.RoleId).Select(u => u.Text).First();

                    //add to role
                    var added = await _userManager.AddToRoleAsync(updatedUser, newRoleName);

                    //assign to selected projects
                    if (SelectedProjects.Any())
                    {
                        int i = 0;

                        //get all project admins
                        List<string> allAdmins = _context.UserRoleModel.Include(r => r.Role).Where(r => r.Role.Name == "Administrator").Select(s => s.UserId).ToList();
                        List<AspNetUserModel> projectAdmins = _context.AspNetUserModel.Where(u => allAdmins.Contains(u.Id)).Include(u => u.UserProjects).ToList();

                        foreach (var project in SelectedProjects)
                        {
                            if (_context.UserProjectModel.Where(p => p.AspNetUserID == updatedUser.Id && p.ProjectId == project).Count() <= 0)
                            {
                                UserProjectModel upm = new UserProjectModel();

                                upm.AspNetUserID = updatedUser.Id;
                                upm.ProjectId = project;
                                upm.CreatedDate = DateTime.Now;
                                upm.CreatedBy = User.Identity.Name;

                                _context.UserProjectModel.Add(upm);
                                await _context.SaveChangesAsync();
                            }

                            //get Project Name
                            if (i > 0)
                            {
                                projectsAssigned += ", ";
                                projectAdminName += ", ";
                                projectAdminEmail += ", ";
                            }

                            projectsAssigned += _context.ProjectModel.Where(p => p.ID == project).FirstOrDefault().Name;

                            //get Project Admin Name and email
                            var thisProjAdmin = projectAdmins.Where(p => p.UserProjects.Where(a => a.ProjectId == project).Count() > 0).FirstOrDefault();
                            if (thisProjAdmin != null)
                            {
                                projectAdminName += thisProjAdmin.FullName; 
                                projectAdminEmail += thisProjAdmin.Email;
                            }
                            i++;
                        }
                    }
                }

                if (result.Succeeded)
                {
                    //set firstname & lastname
                    var usermodel = await _context.AspNetUserModel.FindAsync(updatedUser.Id);

                    if (usermodel != null)
                    {
                        usermodel.FirstName = UserModel.FirstName;
                        usermodel.LastName = UserModel.LastName;
                        usermodel.LockoutEnabled = false;
                        _context.AspNetUserModel.Update(usermodel);
                        await _context.SaveChangesAsync();
                    }

                    _logger.LogInformation("Created a new user account. User will need to use the Forgot Password feature to login.");

                    //get link to set password
                    var code = await _userManager.GeneratePasswordResetTokenAsync(updatedUser);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);
                    //end link generation

                    //Email user
                    await _emailSender.SendEmailAsync(UserModel.Email,
                    $"ObserveAssign - {projectsAssigned} New Account Created",
                    $"<p>Hello {UserModel.FullName},</p><p>You now have access to the Remote Observation through Video in Education Research (ObserveAssign) platform for the {projectsAssigned} Project. This platform will be used to access and view videos for the {projectsAssigned} Project. To use the platform please login with your credentials at {this.Request.Scheme}://{this.Request.Host} or set your initial password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. Your assigned videos will show up on the home page. Please view and code the videos using your assigned Classroom Observation tool. You will not have the ability to pause, fast forward, or rewind the video and you can only replay it once. Please complete the viewing and coding of the video at one time, don’t start a video and come back to it later. Once you have completed your coding, please press the “Complete” button and move onto the next video. If you have any questions or issues, please contact {projectAdminName} at {projectAdminEmail}.</p><p>Thank you,</p><p>{projectAdminName}</p>");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    return RedirectToPage("./Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                //return user back to page
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
