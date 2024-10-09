using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Project
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public IndexModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }

        public IList<ProjectModel> ProjectModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.ProjectModel != null)
            {
                ProjectModel = await _context.ProjectModel.ToListAsync();
            }
        }
    }
}
