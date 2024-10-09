using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Data;
using ObserveAssign.Models;
using System.Data;

namespace ObserveAssign.Pages.User
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ObserveAssignDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(ObserveAssignDbContext context
            , UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

            SetLookups();
        }

        private void SetLookups()
        {
            var userRoles = _roleManager.Roles.ToList();

            UserRoles = new SelectList(userRoles, "Id", "Name");

            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.ToList();

                ProjectList = new SelectList(projects, "ID", "Name");
            }
        }

        [BindProperty]
        public AspNetUserModel UserModel { get; set; } = default!;
        public SelectList UserRoles { get; set; }
        public SelectList ProjectList { get; set; }
        [BindProperty]
        public UserRoleModel CurrentUserRole { get; set; }
        [BindProperty]
        public List<int> SelectedProjects { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null || _context.AspNetUserModel == null)
            {
                return NotFound();
            }

            var userdetails = await _context.AspNetUserModel.FirstOrDefaultAsync(m => m.ID == id);
            if (userdetails == null)
            {
                return NotFound();
            }

            //set user role
            userdetails.UserRoles = _context.UserRoleModel.Where(a => a.UserId == userdetails.ID).ToList();
            if (userdetails.UserRoles.Count > 0)
            {
                CurrentUserRole = userdetails.UserRoles.First();
            }

            //set assigned projects
            SelectedProjects = _context.UserProjectModel.Where(p => p.AspNetUserID == userdetails.ID).Select(p => p.ProjectId).ToList();

            UserModel = userdetails;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            UserModel.UserRoles = _context.UserRoleModel.Where(a => a.UserId == UserModel.ID).ToList();
            UserModel.UserProjects = _context.UserProjectModel.Where(p => p.AspNetUserID == UserModel.ID).ToList();
            IdentityUser updatedUser = _userManager.Users.Where(u => u.Id == UserModel.ID).FirstOrDefault();

            //set newly selected user role - changed
            if (UserModel.UserRoles.Where(u => u.RoleId == CurrentUserRole.RoleId).Count() <= 0)
            {
                if (updatedUser != null)
                {
                    string newRoleName = UserRoles.Where(u => u.Value == CurrentUserRole.RoleId).Select(u => u.Text).First();

                    foreach (var role in _roleManager.Roles.ToList())
                    {
                        //add to role
                        if (CurrentUserRole.RoleId == role.Id)
                        {
                            var added = await _userManager.AddToRoleAsync(updatedUser, newRoleName);
                        }
                        else
                        {
                            //remove
                            var removed = await _userManager.RemoveFromRoleAsync(updatedUser, role.Name);
                        }
                    }
                }
            }

            //assign to selected projects
            if (SelectedProjects.Any())
            {
                //check existing project assignments
                if (UserModel.UserProjects != null)
                {
                    foreach (UserProjectModel existingProject in UserModel.UserProjects)
                    {
                        //remove from project
                        if (!SelectedProjects.Contains(existingProject.ProjectId))
                        {
                            _context.UserProjectModel.Remove(existingProject);
                            await _context.SaveChangesAsync();
                        }
                    }

                    foreach (int project in SelectedProjects)
                    {
                        //add to project if doesn't already exist
                        if (UserModel.UserProjects.Where(u => u.ProjectId == project).Count() <= 0)
                        {
                            UserProjectModel upm = new UserProjectModel();

                            upm.AspNetUserID = UserModel.ID;
                            upm.ProjectId = project;
                            upm.CreatedDate = DateTime.Now;
                            upm.CreatedBy = User.Identity.Name;

                            _context.UserProjectModel.Add(upm);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    //no project assigned - only additions
                    foreach (int project in SelectedProjects)
                    {
                        UserProjectModel upm = new UserProjectModel();

                        upm.AspNetUserID = UserModel.ID;
                        upm.ProjectId = project;
                        upm.CreatedDate = DateTime.Now;
                        upm.CreatedBy = User.Identity.Name;

                        _context.UserProjectModel.Add(upm);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            //if (!ModelState.IsValid)
            //{
            //return Page();
            //}

            //update other data fields, but keep items like password, etc not shown on user interface
            AspNetUserModel updatedUserModel = _context.AspNetUserModel.Where(u => u.ID == UserModel.ID).FirstOrDefault();

            updatedUserModel.UserName = UserModel.UserName;
            updatedUserModel.NormalizedUserName = UserModel.UserName.ToUpper();
            updatedUserModel.FirstName = UserModel.FirstName;
            updatedUserModel.LastName = UserModel.LastName;
            updatedUserModel.Email = UserModel.Email;
            updatedUserModel.NormalizedEmail = UserModel.Email.ToUpper();
            updatedUserModel.LockoutEnabled = UserModel.LockoutEnabled;

            _context.Attach(updatedUserModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserModelExists(UserModel.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserModelExists(string id)
        {
            return _context.AspNetUserModel.Any(e => e.ID == id);
        }
    }
}
