using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("user_project")]
    public class UserProjectModel : TableLoggingModel
    {
        public string AspNetUserID { get; set; }
        public int ProjectId { get; set; }

        //Foreign Keys
        public ProjectModel Project { get; set; }
        public AspNetUserModel AspNetUser { get; set; }
    }
}
