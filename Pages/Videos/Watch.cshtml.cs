using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Pages.Videos
{
    [Authorize(Roles = "Video Viewer, Administrator")]
    public class WatchModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;

        public WatchModel(ObserveAssign.Data.ObserveAssignDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [BindProperty]
        public VideoModel VideoModel { get; set; } = default!;

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }
        public SelectList Projects { get; set; }
        public SelectList Schools { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.VideoModel == null)
            {
                return NotFound();
            }

            var uservideo = await _context.UserVideoModel.Include(v => v.Video).FirstOrDefaultAsync(m => m.Id == id);
            if (uservideo == null)
            {
                return NotFound();
            }

            UserVideoModel = uservideo;

            VideoModel = uservideo.Video; // await _context.VideoModel.FirstOrDefaultAsync(m => m.ID == uservideo.VideoId);
            if (VideoModel == null)
            {
                return NotFound();
            }

            //if somehow the user saved the URL - and already saw the video - but it is locked - redirect them back home
            if (UserVideoModel.IsLocked)
            {
                return RedirectToPage("/Home/Index");
            }

            ////Increment the watch count
            //UserVideoModel.NumberOfWatches = UserVideoModel.NumberOfWatches + 1;
            ////if number of watches exceeds the number of allowed views - lock it.
            //if (UserVideoModel.NumberOfWatches >= VideoModel.NumberViewsAllowed)
            //{
            //    UserVideoModel.IsLocked = true;
            //}

            //_context.Attach(UserVideoModel).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!UserVideoModelExists(UserVideoModel.Id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            VideoModel.URL = Services.CloudFrontStreaming.getSignedURL(VideoModel.URL, _configuration);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (UserVideoModel.Video == null)
            {
                //ignore errors related to FK objects not being loaded - OK, not changing those links here
                ModelState.Remove("UserVideoModel.Video");
                ModelState.Remove("UserVideoModel.Tool");
                ModelState.Remove("UserVideoModel.AspNetUser");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Sanitize HTML for Notes - and save properly to the database. Then when pulling back, make sure pulls as < instead of &lt;


            UserVideoModel.LastUpdatedDate = DateTime.Now;
            UserVideoModel.LastUpdatedBy = User.Identity.Name;

            _context.Attach(UserVideoModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserVideoModelExists(UserVideoModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect("/Home/Index");
        }

        private bool UserVideoModelExists(int id)
        {
            return _context.UserVideoModel.Any(e => e.Id == id);
        }
    }
}
