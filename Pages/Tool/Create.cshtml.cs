using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Tool
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;

        public CreateModel(ObserveAssign.Data.ObserveAssignDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public ToolModel ToolModel { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ToolModel.CreatedDate = DateTime.Now;
            ToolModel.CreatedBy = User.Identity.Name;

            _context.ToolModel.Add(ToolModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
