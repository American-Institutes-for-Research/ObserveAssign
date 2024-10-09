using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.UserVideo
{
    [Authorize(Roles = "Administrator, Video Viewer")]
    public class EditModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ObserveAssign.Data.ObserveAssignDbContext context
            , IConfiguration configuration
            , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            SetLookups();
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (_context.UserVideoModel == null)
            {
                return NotFound();
            }

            var uservideo = await _context.UserVideoModel.FirstOrDefaultAsync(m => m.Id == id);
            if (uservideo == null)
            {
                return NotFound();
            }

            UserVideoModel = uservideo;

            return Page();
        }
        private void SetLookups()
        {
            if (_context.AspNetUserModel != null)
            {
                var users = _context.AspNetUserModel.ToList();

                Users = new SelectList(users, "ID", "UserName");
            }
            if (_context.VideoModel != null)
            {
                var videos = _context.VideoModel.ToList();
                Videos = new SelectList(videos, "ID", "Name");
            }
            if (_context.ToolModel != null)
            {
                var tools = _context.ToolModel.ToList();
                Tools = new SelectList(tools, "Id", "Name");
            }
        }

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }
        public SelectList Users { get; set; }
        public SelectList Videos { get; set; }
        public SelectList Tools { get; set; }
        public List<string> errorMessages { get; set; } = new List<string>();


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //check for errors on nullable fields
            errorMessages = validateGetErrors();

            if (errorMessages.Count > 0)
            {
                //return user back to page
                return Page();
            }

            UserVideoModel.LastUpdatedDate = DateTime.Now;
            UserVideoModel.LastUpdatedBy = User.Identity.Name;

            _context.Attach(UserVideoModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return RedirectToPage("./Index");
        }

        private bool UserVideoModelExists(int id)
        {
            return _context.UserVideoModel.Any(e => e.Id == id);
        }

        private List<string> validateGetErrors()
        {
            if (string.IsNullOrEmpty(UserVideoModel.AspNetUserID) || UserVideoModel.AspNetUserID == "-1")
            {
                errorMessages.Add("User is required");
            }
            if (UserVideoModel == null || UserVideoModel.VideoId == -1)
            {
                errorMessages.Add("Video is required");
            }
            if (UserVideoModel == null || UserVideoModel.ToolID == -1)
            {
                errorMessages.Add("Tool is required");
            }

            return errorMessages;
        }
    }
}
