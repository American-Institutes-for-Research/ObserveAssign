using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.UserVideo
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public DeleteModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (_context.UserVideoModel == null)
            {
                return NotFound();
            }

            var uservideo = await _context.UserVideoModel.Include(m => m.AspNetUser).Include(m => m.Video).FirstOrDefaultAsync(m => m.Id == id);

            if (uservideo == null)
            {
                return NotFound();
            }
            else
            {
                UserVideoModel = uservideo;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (_context.UserVideoModel == null)
            {
                return NotFound();
            }
            var uservideo = await _context.UserVideoModel.FindAsync(id);

            if (uservideo != null)
            {
                UserVideoModel = uservideo;
                _context.UserVideoModel.Remove(UserVideoModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
