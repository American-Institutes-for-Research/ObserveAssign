using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.School
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;

        public IndexModel(ObserveAssign.Data.ObserveAssignDbContext context)
        {
            _context = context;
        }

        public IList<SchoolModel> SchoolModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.SchoolModel != null)
            {
                SchoolModel = await _context.SchoolModel.ToListAsync();
            }
        }
    }
}
