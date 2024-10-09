using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("teachers")]
    public class TeacherModel
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Project")]
        public int ProjectID { get; set; }

        //Foreign Keys
        public ProjectModel Project { get; set; }
    }
}
