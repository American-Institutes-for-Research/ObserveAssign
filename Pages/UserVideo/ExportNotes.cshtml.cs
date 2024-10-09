using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.UserVideo
{
    [Authorize(Roles = "Video Viewer, Administrator")]
    public class ExportModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;

        public ExportModel(ObserveAssign.Data.ObserveAssignDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [BindProperty]
        public VideoModel VideoModel { get; set; } = default!;

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.VideoModel == null)
            {
                return NotFound();
            }

            var uservideo = await _context.UserVideoModel.FirstOrDefaultAsync(m => m.Id == id);
            if (uservideo == null)
            {
                return NotFound();
            }

            UserVideoModel = uservideo;

            var videomodel = await _context.VideoModel.FirstOrDefaultAsync(m => m.ID == uservideo.VideoId);
            if (videomodel == null)
            {
                return NotFound();
            }
            VideoModel = videomodel;

            return Page();
        }
    }
}
