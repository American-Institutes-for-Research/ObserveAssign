using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("aspnetusers")]
    public class AspNetUserModel : IdentityUser
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
        public List<UserRoleModel> UserRoles { get; set; }

        [Display(Name = "Project")]
        public List<UserProjectModel> UserProjects { get; set; }
        //public List<UserVideoModel> UserVideos { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName ?? string.Empty, LastName ?? string.Empty);
            }
        }
    }
}
