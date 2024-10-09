using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ObserveAssign.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string RoleId { get; set; }
    }
}
