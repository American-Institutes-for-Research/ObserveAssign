using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ObserveAssign.Models;
using ObserveAssign.Services;

namespace ObserveAssign.Pages.Videos
{
    [RequestSizeLimit(1090519040)] //limits to 1 GB so can get into page. shows 400 error to users if triggered
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private const int MAXVIDEOLENGTH = 524288000; // Set the limit to 500 MB

        public CreateModel(ObserveAssign.Data.ObserveAssignDbContext context, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;

            SetLookups();
        }

        public IActionResult OnGet()
        {
            var user = _userManager.GetUserAsync(User).Result;

            //only get list of assigned projects
            var SelectedProjects = _context.UserProjectModel.Where(p => p.AspNetUserID == user.Id).Select(p => p.ProjectId).ToList();

            //set Project lookup
            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.Where(p => SelectedProjects.Contains(p.ID)).ToList();

                Projects = new SelectList(projects, "ID", "Name");
            }
            return Page();
        }

        private void SetLookups()
        {
            List<int> SelectedProjects = new List<int>();
            if (User != null) { 
                var user = _userManager.GetUserAsync(User).Result;

                //only get list of assigned projects
                SelectedProjects = _context.UserProjectModel.Where(p => p.AspNetUserID == user.Id).Select(p => p.ProjectId).ToList();
            }
            //set Project lookup
            if (_context.ProjectModel != null)
            {
                var projects = _context.ProjectModel.Where(p => SelectedProjects.Contains(p.ID)).ToList();

                Projects = new SelectList(projects, "ID", "Name");
            }

            if (_context.SchoolModel != null)
            {
                var schools = _context.SchoolModel.ToList();

                Schools = new SelectList(schools, "ID", "Name");
            }
        }

        [BindProperty]
        public VideoModel VideoModel { get; set; }

        public SelectList Projects { get; set; }
        public SelectList Schools { get; set; }

        public string errorMessage { get; set; }

        /// <summary>
        /// Currently only allows MP4's - if any other file extension or content type - fails
        /// </summary>
        /// <returns>True if a valid video/mp4 file - false otherwise</returns>
        private bool isValidContentType()
        {
            if (VideoModel.VideoFile.ContentType != "video/mp4")
            {
                return false;
            }
            string extension = VideoModel.VideoFile.FileName.Split('.').Last();

            List<string> validExtensions = new List<string>() { "mp4" };

            if (!validExtensions.Contains(extension))
            {
                return false;
            }

            //passes all checks
            return true;
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //upload to AWS S3 Bucket
            //Consider streaming: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0#upload-large-files-with-streaming
            if (VideoModel.VideoFile != null && isValidContentType())
            {
                if(VideoModel.VideoFile.Length > MAXVIDEOLENGTH)
                {
                    errorMessage = "The video file is too large. Files must be less than 500 MB.";
                    SetLookups();
                    return Page();
                }

                string fileNameOrError = UploadToTempFolder(VideoModel.VideoFile);
                if(fileNameOrError == null || fileNameOrError == "Error uploading file")
                {
                    errorMessage = "Error uploading file";
                    SetLookups();
                    return Page();
                }
                string url = AWSTasks.UploadToS3FromLocalFile(fileNameOrError, VideoModel.VideoFile.FileName, _configuration);
                //string url = AWSTasks.UploadToS3(VideoModel.VideoFile, VideoModel.VideoFile.FileName, _configuration);
                //string url = AWSTasks.UploadMultiPartS3(VideoModel.VideoFile, VideoModel.VideoFile.FileName, _configuration);

                VideoModel.URL = url;
            }
            else
            {
                errorMessage = "A valid video file must be provided";
                SetLookups();
                //return user back to page
                return Page();
            }

            VideoModel.CreatedDate = DateTime.Now;
            VideoModel.CreatedBy = User.Identity.Name;

            _context.VideoModel.Add(VideoModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// From .net examples - for uploading large files https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/5.x/LargeFilesSample/Controllers/FileUploadController.cs
        /// </summary>
        /// <returns>The file name or error if it cannot be uploaded properly</returns>
        private string UploadToTempFolder(IFormFile videoFile)
        {
            //use temp folder within project structure
            string path = Path.GetTempPath(); 

            try
            {
                // Get the temporary folder, and combine a random file name with it
                string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".mp4";
                string fullPath = Path.Combine(path, fileName);

                using (FileStream targetStream = System.IO.File.Create(fullPath))
                {
                    videoFile.CopyTo(targetStream);
                }

                return fullPath;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Error uploading file";
            }
        }
    }
}
