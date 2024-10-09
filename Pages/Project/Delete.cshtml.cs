using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Project
{
    public class DeleteModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public DeleteModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProjectModel ProjectModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (_context.ProjectModel == null)
            {
                return NotFound();
            }

            var project = await _context.ProjectModel.FirstOrDefaultAsync(m => m.ID == id);

            if (project == null)
            {
                return NotFound();
            }
            else
            {
                ProjectModel = project;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (_context.ProjectModel == null)
            {
                return NotFound();
            }
            var project = await _context.ProjectModel.FindAsync(id);

            if (project != null)
            {
                ProjectModel = project;
                _context.ProjectModel.Remove(ProjectModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
