using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Project
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ProjectModel ProjectModel { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ProjectModel.CreatedDate = DateTime.Now;
            ProjectModel.CreatedBy = User.Identity.Name;

            _context.ProjectModel.Add(ProjectModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
