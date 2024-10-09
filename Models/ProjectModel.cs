using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("projects")]
    public class ProjectModel : TableLoggingModel
    {
        [Key]
        public int ID { get; set; }

        public string? Name { get; set; }
    }
}
