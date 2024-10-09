using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.School
{
    public class DeleteModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public DeleteModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SchoolModel SchoolModel { get; set; }

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
            else
            {
                SchoolModel = school;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (_context.SchoolModel == null)
            {
                return NotFound();
            }
            var school = await _context.SchoolModel.FindAsync(id);

            if (school != null)
            {
                SchoolModel = school;
                _context.SchoolModel.Remove(SchoolModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
