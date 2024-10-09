using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Videos
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
        public VideoModel VideoModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (_context.VideoModel == null)
            {
                return NotFound();
            }

            var videomodel = await _context.VideoModel.FirstOrDefaultAsync(m => m.ID == id);

            if (videomodel == null)
            {
                return NotFound();
            }
            else
            {
                VideoModel = videomodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.VideoModel == null)
            {
                return NotFound();
            }
            var videomodel = await _context.VideoModel.FindAsync(id);

            if (videomodel != null)
            {
                VideoModel = videomodel;
                _context.VideoModel.Remove(VideoModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
