using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.UserVideo
{
    [Authorize(Roles = "Video Viewer, Administrator")]
    public class WatchModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public WatchModel(ObserveAssign.Data.ObserveAssignDbContext context, IConfiguration configuration, IEmailSender emailSender)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.VideoModel == null)
            {
                return NotFound();
            }

            UserVideoModel = await _context.UserVideoModel.Include(v => v.Video).FirstOrDefaultAsync(m => m.Id == id);
            if (UserVideoModel == null)
            {
                return NotFound();
            }

            UserVideoModel.LastUpdatedDate = DateTime.Now;
            UserVideoModel.LastUpdatedBy = User.Identity.Name;
            UserVideoModel.RewatchRequested = true;

            _context.Attach(UserVideoModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                //email Project Administrator
                EmailProjectAdministrator();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserVideoModelExists(UserVideoModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Page();
        }

        private bool UserVideoModelExists(int id)
        {
            return _context.UserVideoModel.Any(e => e.Id == id);
        }

        private async void EmailProjectAdministrator()
        {
            //get all project admins
            ProjectModel projectsAssigned = _context.ProjectModel.Where(p => p.ID == UserVideoModel.Video.ProjectId).FirstOrDefault();
            string projectAdminName = string.Empty;
            string projectAdminEmail = string.Empty;

            List<string> allAdmins = _context.UserRoleModel.Include(r => r.Role).Where(r => r.Role.Name == "Administrator").Select(s => s.UserId).ToList();
            List<AspNetUserModel> projectAdmins = _context.AspNetUserModel.Where(u => !u.LockoutEnabled && allAdmins.Contains(u.Id)).Include(u => u.UserProjects).ToList();

            //get project admin name and email
            var thisProjAdmin = projectAdmins.Where(p => p.UserProjects.Where(a => a.ProjectId == UserVideoModel.Video.ProjectId).Count() > 0).FirstOrDefault();
            if (thisProjAdmin != null)
            {
                projectAdminName += thisProjAdmin.FullName;
                projectAdminEmail += thisProjAdmin.Email;
            }

            //Email user
            await _emailSender.SendEmailAsync(projectAdminEmail,
            $"ObserveAssign - Watch Again Request",
            $"<p>Hello {projectAdminName},</p><p>A user has requested to rewatch a video in ObserveAssign. Please see the request below.</p><p>{UserVideoModel.AspNetUser.FullName} requested to watch the video: {UserVideoModel.Video.Name} again</p>");

        }
    }
}
