using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ObserveAssign.Models;
using System.Data;
using System.Linq;

namespace ObserveAssign.Pages.User
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ObserveAssign.Data.ObserveAssignDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public SelectList Projects { get; set; }
        [BindProperty]
        public int SelectedProjectID { get; set; }

        public IList<AspNetUserViewModel> UserModel { get; set; } = default!;
        public List<IdentityRole> Roles { get; set; }
        public List<UserVideoModel> UserVideos { get; set; }

        public async Task OnGetAsync()
        {
            var user = _userManager.GetUserAsync(User).Result;
            //only get list of assigned projects
            var SelectedProjects = _context.UserProjectModel.Where(p => p.AspNetUserID == user.Id).Select(p => p.ProjectId).ToList();
            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.Where(p => SelectedProjects.Contains(p.ID)).ToList();

                Projects = new SelectList(projects, "ID", "Name");
            }

            if (_context.AspNetUserModel != null)
            {
                //only show users that are in the projects associated with this user
                IList<AspNetUserModel> EFUserModel = await _context.AspNetUserModel.Include(m => m.UserRoles).Include(m => m.UserProjects).ToListAsync();

                //get user roles
                Roles = _roleManager.Roles.ToList();

                //get videos assigned to users
                UserVideos = _context.UserVideoModel.Include(v => v.Video).Include(s => s.Video.School).Include(s => s.Video.Project).ToList();

                UserModel = new List<AspNetUserViewModel>();
                foreach(var EFUser in EFUserModel)
                {
                    UserModel.Add(new AspNetUserViewModel(EFUser, Roles, UserVideos));
                }

                IList<AspNetUserViewModel> restrictedByProject = new List<AspNetUserViewModel>();

                foreach (int projID in SelectedProjects)
                {
                    restrictedByProject.AddRange(UserModel.Where(p => p.UserProjects.Select(u => u.ProjectId).Contains(projID)).ToList());
                }
                UserModel = restrictedByProject.Distinct().ToList();
            }
        }
    }
}
