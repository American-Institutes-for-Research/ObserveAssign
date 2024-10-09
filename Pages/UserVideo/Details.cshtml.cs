using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.UserVideo
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public DetailsModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (_context.UserVideoModel == null)
            {
                return NotFound();
            }

            var uservideo = await _context.UserVideoModel.Include(m => m.AspNetUser).Include(m => m.Tool).Include(m => m.Video).FirstOrDefaultAsync(m => m.Id == id);
            if (uservideo == null)
            {
                return NotFound();
            }

            UserVideoModel = uservideo;

            return Page();
        }

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }
    }
}
