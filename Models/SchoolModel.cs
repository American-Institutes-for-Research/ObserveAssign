using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("schools")]
    public class SchoolModel : TableLoggingModel
    {
        [Key]
        public string ID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        //Foreign Keys
        //public ProjectModel Project { get; set; }
    }
}
