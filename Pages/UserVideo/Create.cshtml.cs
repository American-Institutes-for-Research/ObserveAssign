using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Linq;

namespace ObserveAssign.Pages.UserVideo
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ObserveAssign.Data.ObserveAssignDbContext _context;
        private IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CreateModel(ObserveAssign.Data.ObserveAssignDbContext context
            , IConfiguration configuration
            , UserManager<IdentityUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _configuration = configuration;
            SetLookups();
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult OnGet(string? id)
        {
            if(id == null)
            {
                return Page();
            }

            UserVideoModel = new UserVideoModel();
            UserVideoModel.AspNetUserID = id;

            return Page();
        }

        private void SetLookups()
        {
            if (_context.AspNetUserModel != null)
            {
                var users = _context.AspNetUserModel.ToList();

                Users = new SelectList(users, "ID", "FullName");
            }
            if (_context.VideoModel != null)
            {
                var videos = _context.VideoModel.ToList();
                Videos = new SelectList(videos, "ID", "Name");
            }
            if (_context.ToolModel != null)
            {
                var tools = _context.ToolModel.ToList();
                Tools = new SelectList(tools, "Id", "Name");
            }
        }

        [BindProperty]
        public UserVideoModel UserVideoModel { get; set; }
        public SelectList Users { get; set; }
        public SelectList Videos { get; set; }
        [BindProperty]
        [Display(Name = "Videos")]
        public List<int> SelectedVideos { get; set; }
        public SelectList Tools { get; set; }
        public List<string> errorMessages { get; set; } = new List<string>();

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //check for errors on nullable fields
            errorMessages = validateGetErrors();

            if (errorMessages.Count > 0)
            {
                //return user back to page
                return Page();
            }

            if (UserVideoModel.Video == null)
            {
                //ignore errors related to FK objects not being loaded - OK
                ModelState.Remove("UserVideoModel.Video");
                ModelState.Remove("UserVideoModel.Tool");
                ModelState.Remove("UserVideoModel.AspNetUser");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ////Check to make sure user wasn't already assigned to this video
            //if(_context.UserVideoModel.Where(u => u.AspNetUserID == UserVideoModel.AspNetUserID && u.VideoId == UserVideoModel.VideoId).Count() > 0)
            //{
            //    errorMessages.Add("The selected user has already been assigned to the selected video.");
            //    return Page();
            //}

            string videosAssigned = string.Empty;

            //assign to selected videos
            if (SelectedVideos.Any())
            {
                int i = 0;

                foreach (var videoId in SelectedVideos)
                {
                    //Check to make sure user wasn't already assigned to this video
                    if (_context.UserVideoModel.Where(p => p.AspNetUserID == UserVideoModel.AspNetUserID && p.VideoId == videoId).Count() <= 0)
                    {
                        UserVideoModel upm = new UserVideoModel();

                        upm.AspNetUserID = UserVideoModel.AspNetUserID;
                        upm.VideoId = videoId;
                        upm.ToolID = UserVideoModel.ToolID;
                        upm.NumberOfWatches = UserVideoModel.NumberOfWatches;
                        upm.IsLocked = UserVideoModel.IsLocked;
                        upm.IsArchived = UserVideoModel.IsArchived;
                        upm.IsComplete = UserVideoModel.IsComplete;
                        upm.IsLocked = UserVideoModel.IsLocked;

                        upm.CreatedDate = DateTime.Now;
                        upm.CreatedBy = User.Identity.Name;

                        var newModel = _context.UserVideoModel.Add(upm);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        errorMessages.Add("The selected user has already been assigned to the selected video.");
                        return Page();
                    }

                    //get Project Name for email
                    if (i > 0)
                    {
                        videosAssigned += "</p><p>";
                    }

                    videosAssigned += _context.VideoModel.Where(p => p.ID == videoId).FirstOrDefault().Name;

                    i++;
                }
            }
            //finish project assignments

            await sendEmailToUserAsync(videosAssigned);

            return RedirectToPage("./Index");
        }

        private async Task sendEmailToUserAsync(string videosAssigned)
        {
            //get all project admins
            try
            {
                if (SelectedVideos.Any())
                {
                    int i = 0;
                    List<string> projectAdmin = new List<string>();
                    List<string> projectAdminNames = new List<string>();
                    string projectsAssigned = string.Empty;
                    string listOfVideosAdmins = string.Empty;

                    List<string> allAdmins = _context.UserRoleModel.Include(r => r.Role).Where(r => r.Role.Name == "Administrator").Select(s => s.UserId).ToList();
                    List<AspNetUserModel> projectAdmins = _context.AspNetUserModel.Where(u => allAdmins.Contains(u.Id)).Include(u => u.UserProjects).ToList();
                    UserVideoModel.AspNetUser = _context.AspNetUserModel.Where(a => a.Id == UserVideoModel.AspNetUserID).FirstOrDefault();

                    foreach (int videoId in SelectedVideos)
                    {
                        //get project admin name and email
                        var thisVideo = _context.VideoModel.Where(v => v.ID == videoId).FirstOrDefault();
                        projectsAssigned = _context.ProjectModel.Where(p => p.ID == thisVideo.ProjectId).FirstOrDefault().Name;
                        var thisProjAdmin = projectAdmins.Where(p => p.UserProjects.Where(a => a.ProjectId == thisVideo.ProjectId).Count() > 0).FirstOrDefault();
                        if (thisProjAdmin != null)
                        {
                            //projectAdmin.Add(thisProjAdmin.FullName + " at " + thisProjAdmin.Email);
                            //projectAdminNames.Add(thisProjAdmin.FullName);
                            listOfVideosAdmins += "<p>" + thisVideo.Name + ": " + thisProjAdmin.FullName + " at " + thisProjAdmin.Email + "</p>";
                        }
                    }
                    
                    //Email user
                    await _emailSender.SendEmailAsync(UserVideoModel.AspNetUser.Email,
                    $"ObserveAssign - {projectsAssigned} New Video Assignment",
                    $"<p>Hello {UserVideoModel.AspNetUser.FullName},</p><p>You have been assigned new videos to code on ObserveAssign. Please see your assignments below. Make sure you view and code videos in the order given and press the “Complete” button when you finish. If you have any questions or issues, the point of contact is listed next to each video below:</p><p>{listOfVideosAdmins}</p><p>Thank you,</p><p>ObserveAssign</p>");
                }
            }
            catch(Exception ex)
            {
                errorMessages.Add("No project administrators are assigned to this project.");
                return;
            }
        }

        private List<string> validateGetErrors()
        {
            if (string.IsNullOrEmpty(UserVideoModel.AspNetUserID) || UserVideoModel.AspNetUserID == "-1")
            {
                errorMessages.Add("User is required");
            }
            if (UserVideoModel == null || UserVideoModel.ToolID == -1)
            {
                errorMessages.Add("Tool is required");
            }

            return errorMessages;
        }
    }
}
