using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(UserManager<IdentityUser> userManager, ObserveAssign.Data.ObserveAssignDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IList<UserProjectModel> UserProjectModel { get; set; } = default!;
        public IList<UserVideoModel> UserVideoModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.LockoutEnabled)
            {
                await _signInManager.SignOutAsync();
                return LocalRedirect("~/Identity/Account/Lockout");
            }

            if (_context.UserProjectModel != null)
            {
                UserProjectModel = await _context.UserProjectModel.Where(u => u.AspNetUserID == user.Id).Include(p => p.Project).ToListAsync();
            }
            if (_context.UserVideoModel != null)
            {
                UserVideoModel = await _context.UserVideoModel.Where(v => v.AspNetUserID == user.Id).Include(p => p.Video).ToListAsync();
            }
            return Page();
        }
    }
}
