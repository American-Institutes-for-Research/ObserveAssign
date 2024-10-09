using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Videos
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ObserveAssign.Data.ObserveAssignDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public SelectList Projects { get; set; }
        [BindProperty]
        public int SelectedProjectID { get; set; }

        public IList<VideoModel> VideoModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var user = _userManager.GetUserAsync(User).Result;
            //only get list of assigned projects
            var SelectedProjects = _context.UserProjectModel.Where(p => p.AspNetUserID == user.Id).Select(p => p.ProjectId).ToList();

            //set Project lookup
            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.Where(p => SelectedProjects.Contains(p.ID)).ToList();

                Projects = new SelectList(projects, "ID", "Name");
            }

            if (_context.VideoModel != null)
            {
                VideoModel = await _context.VideoModel.Where(v => SelectedProjects.Contains(v.ProjectId)).ToListAsync();
                //.Include(v => v.Project)
            }
        }
    }
}
