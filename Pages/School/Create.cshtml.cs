using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.School
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
            SetProjects();
        }

        public IActionResult OnGet()
        {
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SchoolModel.CreatedDate = DateTime.Now;
            SchoolModel.CreatedBy = User.Identity.Name;

            _context.SchoolModel.Add(SchoolModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
