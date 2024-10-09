using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;
using System.Data;

namespace ObserveAssign.Pages.User
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DetailsModel(ObserveAssign.Data.ObserveAssignDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public AspNetUserModel UserModel { get; set; }
        public List<UserVideoModel> UserVideoModel { get; set; }
        public List<ProjectModel> Projects { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null || _context.AspNetUserModel == null)
            {
                return NotFound();
            }

            var usermodel = await _context.AspNetUserModel.Include(a => a.UserProjects).FirstOrDefaultAsync(m => m.ID == id);
            if (usermodel == null)
            {
                return NotFound();
            }
            else
            {
                UserModel = usermodel;

                //get user role
                UserModel.UserRoles = _context.UserRoleModel.Where(a => a.UserId == UserModel.ID).ToList();
                //get friendly name for display
                foreach (var role in UserModel.UserRoles.Where(r => r.Role == null))
                {
                    role.Role = new RoleModel();
                    role.Role.Name = _roleManager.Roles.Where(rm => rm.Id == role.RoleId).First().Name;
                }

                //get projects
                Projects = _context.ProjectModel.ToList();

                //get videos assigned to use
                UserVideoModel = _context.UserVideoModel.Include(v => v.Video).Include(s => s.Video.School).Include(s => s.Video.Project).Where(v => v.AspNetUserID == id).ToList();
            }
            return Page();
        }
    }
}
