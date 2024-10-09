using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Project
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public EditModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }
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

            ProjectModel = project;

            return Page();
        }

        [BindProperty]
        public ProjectModel ProjectModel { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ProjectModel.LastUpdatedDate = DateTime.Now;
            ProjectModel.LastUpdatedBy = User.Identity.Name;

            _context.Attach(ProjectModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectModelExists(ProjectModel.ID))
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

        private bool ProjectModelExists(int id)
        {
            return _context.ProjectModel.Any(e => e.ID == id);
        }
    }
}
