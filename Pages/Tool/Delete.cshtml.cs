using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Tool
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
        public ToolModel ToolModel { get; set; }

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
            else
            {
                ToolModel = tool;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (_context.ToolModel == null)
            {
                return NotFound();
            }
            var tool = await _context.ToolModel.FindAsync(id);

            if (tool != null)
            {
                ToolModel = tool;
                _context.ToolModel.Remove(ToolModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
