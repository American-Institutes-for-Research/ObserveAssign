using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("tools")]
    public class ToolModel : TableLoggingModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
