using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumForum.Models
{
    public class Discussion
    {
        [Display(Name = "Discussion ID")]
        public int DiscussionId { get; set; }

        [Display(Name = "Discussion Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Discussion Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Image Name")]
        public string ImageFilename { get; set; } = string.Empty;

        [Display(Name = "Creation Date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "List of Comments")]
        public List<Comment> Comments { get; set; } = []; // navigation property

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; } // not stored in database
    }
}
