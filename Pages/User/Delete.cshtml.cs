using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.User
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
        public AspNetUserModel UserModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null || _context.AspNetUserModel == null)
            {
                return NotFound();
            }

            var usermodel = await _context.AspNetUserModel.FirstOrDefaultAsync(m => m.ID == id);

            if (usermodel == null)
            {
                return NotFound();
            }
            else
            {
                UserModel = usermodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (id == null || _context.AspNetUserModel == null)
            {
                return NotFound();
            }
            var usermodel = await _context.AspNetUserModel.FindAsync(id);

            if (usermodel != null)
            {
                UserModel = usermodel;
                _context.AspNetUserModel.Remove(UserModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
