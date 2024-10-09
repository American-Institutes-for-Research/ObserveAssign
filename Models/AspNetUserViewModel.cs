using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    public class AspNetUserViewModel : IdentityUser
    {
        [Key]
        public string ID { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public override string? UserName { get; set; }

        [StringLength(255, MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; } = string.Empty;

        [StringLength(255, MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public override string? Email { get; set; }

        [Required]
        [Display(Name = "Email Confirmed")]
        public override bool EmailConfirmed { get; set; }

        [Display(Name = "Locked Out")]
        public override bool LockoutEnabled { get; set; }

        [Display(Name = "Role")]
        public List<UserRoleViewModel> UserRoles { get; set; }

        [Display(Name = "Project")]
        public List<UserProjectViewModel> UserProjects { get; set; }
        //public List<UserVideoModel> UserVideos { get; set; }
        public string ProjectIds { get; set; }
        public string UserRoleList { get; set; }
        public int NumberAssigned { get; set; }
        public string AssignedSchoolIDs { get; set; }

        public AspNetUserViewModel(AspNetUserModel user, List<IdentityRole> roles, List<UserVideoModel> UserVideos)
        {
            this.ID= user.ID;
            this.UserName= user.UserName;
            this.FirstName= user.FirstName;
            this.LastName = user.LastName;
            this.Email= user.Email;
            this.EmailConfirmed= user.EmailConfirmed;
            this.LockoutEnabled= user.LockoutEnabled;
            this.UserRoles = new List<UserRoleViewModel>();

            foreach (var u in user.UserRoles)
            {
                this.UserRoles.Add(new UserRoleViewModel() { 
                    RoleId= u.RoleId,
                    UserId= u.UserId
                });

                UserRoleList += roles.Where(r => r.Id == u.RoleId).FirstOrDefault().Name + ", ";
            }
            UserRoleList = UserRoleList.TrimEnd().TrimEnd(',');
            this.UserProjects = new List<UserProjectViewModel>(); 
            foreach(var p in user.UserProjects)
            {
                this.UserProjects.Add(new UserProjectViewModel()
                {
                    AspNetUserID = p.AspNetUserID,
                    ProjectId = p.ProjectId
                });
                this.ProjectIds += p.ProjectId.ToString() + "_";
            }

            this.NumberAssigned = UserVideos.Where(v => v.AspNetUserID == user.ID).Count();
            this.AssignedSchoolIDs = string.Empty;
            foreach (string schoolId in UserVideos.Where(v => v.AspNetUserID == user.ID).Select(v => v.Video.SchoolId).Distinct())
            {
                //this.NumberAssigned += UserVideos.Where(v => v.AspNetUserID == user.ID && v.Video.SchoolId == schoolId).Count() + ", ";
                this.AssignedSchoolIDs += schoolId + ", ";
            }
            //this.NumberAssigned = this.NumberAssigned.TrimEnd().TrimEnd(',');
            this.AssignedSchoolIDs = this.AssignedSchoolIDs.TrimEnd().TrimEnd(',');
        }
    }
}
