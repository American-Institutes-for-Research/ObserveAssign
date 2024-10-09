using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("uservideo")]
    public class UserVideoModel : TableLoggingModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "User")]
        public string AspNetUserID { get; set; }
        [Display(Name = "Video")]
        public int VideoId { get; set; }

        public string? Notes { get; set; }
        [Display(Name = "Number of Watches")]
        public int NumberOfWatches { get; set; }
        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; }
        [Display(Name = "Email Notification Date")]
        public DateTime EmailNotificationDate { get; set; }
        [Display(Name = "Is Complete")]
        public bool IsComplete { get; set; }
        [Display(Name = "Tool")]
        public int ToolID { get; set; }
        public bool RewatchRequested { get; set; }

        //Foreign Keys
        public AspNetUserModel AspNetUser { get; set; }
        public VideoModel Video { get; set; }
        public ToolModel Tool { get; set; }
    }
}
