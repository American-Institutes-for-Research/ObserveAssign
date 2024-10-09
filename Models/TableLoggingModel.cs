using System.ComponentModel.DataAnnotations;

namespace ObserveAssign.Models
{
    public class TableLoggingModel
    {
        [Display(Name = "Is Archived")]
        public bool IsArchived { get; set; }
        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        [Display(Name = "Last Updated By")]
        public string? LastUpdatedBy { get; set; }
        [Display(Name = "Last Updated Date")]
        public DateTime? LastUpdatedDate { get; set; }
    }
}
