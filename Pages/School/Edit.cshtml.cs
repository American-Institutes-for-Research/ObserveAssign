using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.School
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public EditModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (_context.SchoolModel == null)
            {
                return NotFound();
            }

            var school = await _context.SchoolModel.FirstOrDefaultAsync(m => m.ID == id);
            if (school == null)
            {
                return NotFound();
            }

            SchoolModel = school;
            SetProjects();

            return Page();
        }

        private void SetProjects()
        {
            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.ToList();

                Projects = new SelectList(projects, "ID", "Name");
            }
        }

        [BindProperty]
        public SchoolModel SchoolModel { get; set; }
        public SelectList Projects { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            SchoolModel.LastUpdatedDate = DateTime.Now;
            SchoolModel.LastUpdatedBy = User.Identity.Name;

            _context.Attach(SchoolModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SchoolModelExists(SchoolModel.ID))
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

        private bool SchoolModelExists(string id)
        {
            return _context.SchoolModel.Any(e => e.ID == id);
        }
    }
}
