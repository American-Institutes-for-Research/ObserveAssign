using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ObserveAssign.Models
{
    [Table("aspnetuserroles")]
    public class UserRoleModel
    {
        public string UserId { get; set; }

        public string RoleId { get; set; }

        //Foreign Keys
        public AspNetUserModel User { get; set; }
        public RoleModel Role { get; set; }

    }
}
