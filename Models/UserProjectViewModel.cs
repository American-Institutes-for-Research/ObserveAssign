using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    public class UserProjectViewModel : TableLoggingModel
    {
        public string AspNetUserID { get; set; }
        public int ProjectId { get; set; }
    }
}
