using System.ComponentModel.DataAnnotations;
using AquariumForum.Data;

namespace AquariumForum.Models
{
    public class Comment
    {
        [Display(Name = "Comment ID")]
        public int CommentId { get; set; } // primary key

        [Display(Name = "Comment")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Creation Date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "Discussion ID")]
        public int DiscussionId { get; set; } // foreign key
        public Discussion? Discussion { get; set; } // navigation property

        [Display(Name = "User ID")]
        public string ApplicationUserId { get; set; } = string.Empty; // foreign key

        public ApplicationUser? ApplicationUser { get; set; } // navigation property
    }
}
