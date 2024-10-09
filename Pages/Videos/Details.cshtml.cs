using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;
using System.Data;

namespace ObserveAssign.Pages.Videos
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;

        public DetailsModel(ObserveAssign.Data.ObserveAssignDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public VideoModel VideoModel { get; set; }
        public List<UserVideoModel> UserVideoModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.VideoModel == null)
            {
                return NotFound();
            }

            var videomodel = await _context.VideoModel.Include(v => v.Project).Include(v => v.School).FirstOrDefaultAsync(m => m.ID == id);
            if (videomodel == null)
            {
                return NotFound();
            }
            else
            {
                VideoModel = videomodel;

                //VideoModel.URL = Services.CloudFrontStreaming.getSignedURL(videomodel.URL, _configuration);
                //DAL.Helpers.CloudFrontStreaming.getVideoType(model.VideoPath);
            }

            //get user video links
            UserVideoModel = _context.UserVideoModel.Where(u => u.VideoId == videomodel.ID).Include(v => v.AspNetUser).ToList();

            return Page();
        }
    }
}
