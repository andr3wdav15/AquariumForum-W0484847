using System.ComponentModel.DataAnnotations;

namespace AquariumForum.Models
{
    public class Comment
    {
        [Display(Name = "Comment ID")]
        public int CommentId { get; set; }

        [Display(Name = "Comment")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Creation Date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "Discussion ID")]
        public int DiscussionId { get; set; } // foreign key
        public Discussion? Discussion { get; set; } // navigation property
    }
}
