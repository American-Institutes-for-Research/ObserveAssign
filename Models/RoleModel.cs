using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("aspnetroles")]
    public class RoleModel
    {
        [Key]
        public string ID { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
