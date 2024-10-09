using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObserveAssign.Models
{
    [Table("videos")]
    public class VideoModel : TableLoggingModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A Project must be selected")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "School")]
        public string? SchoolId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Number of Views Allowed")]
        public int NumberViewsAllowed { get; set; }

        public string? URL { get; set; }

        [Display(Name = "Replay Video")]
        public bool CanRewatch { get; set; }

        [Display(Name = "Pause Video")]
        public bool CanPause { get; set; }

        [Display(Name = "Fast Forward/Rewind Video")]
        public bool CanRewind { get; set; }

        //Video File
        [NotMapped]
        [Display(Name = "Upload Video")]
        public IFormFile? VideoFile { get; set; }

        //Foreign Keys
        public ProjectModel? Project { get; set; }

        public SchoolModel? School { get; set; }
    }
}
