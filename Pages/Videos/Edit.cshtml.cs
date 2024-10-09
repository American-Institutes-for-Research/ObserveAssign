using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Videos
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public EditModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
            SetLookups();
        }

        private void SetLookups()
        {
            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.ToList();

                Projects = new SelectList(projects, "ID", "Name");
            }
            if (_context.SchoolModel != null)
            {
                var schools = _context.SchoolModel.ToList();

                Schools = new SelectList(schools, "ID", "Name");
            }
        }

        [BindProperty]
        public VideoModel VideoModel { get; set; } = default!;
        public SelectList Projects { get; set; }
        public SelectList Schools { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.VideoModel == null)
            {
                return NotFound();
            }

            var videomodel = await _context.VideoModel.FirstOrDefaultAsync(m => m.ID == id);
            if (videomodel == null)
            {
                return NotFound();
            }
            VideoModel = videomodel;
            //ViewData["ProjectId"] = new SelectList(_context.ProjectModel, "ID", "ID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            VideoModel.LastUpdatedDate = DateTime.Now;
            VideoModel.LastUpdatedBy = User.Identity.Name;
            if(VideoModel.SchoolId == "null")
            {
                VideoModel.SchoolId = null;
            }

            _context.Attach(VideoModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoModelExists(VideoModel.ID))
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

        private bool VideoModelExists(int id)
        {
            return _context.VideoModel.Any(e => e.ID == id);
        }
    }
}
