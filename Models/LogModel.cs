using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("logs")]
    public class LogModel
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
    }
}
