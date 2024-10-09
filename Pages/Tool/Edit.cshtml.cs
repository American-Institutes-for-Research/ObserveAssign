using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Tool
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
            if (_context.ToolModel == null)
            {
                return NotFound();
            }

            var tool = await _context.ToolModel.FirstOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            ToolModel = tool;

            return Page();
        }

        [BindProperty]
        public ToolModel ToolModel { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ToolModel.LastUpdatedDate = DateTime.Now;
            ToolModel.LastUpdatedBy = User.Identity.Name;

            _context.Attach(ToolModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToolModelExists(ToolModel.Id))
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

        private bool ToolModelExists(int id)
        {
            return _context.ToolModel.Any(e => e.Id == id);
        }
    }
}
