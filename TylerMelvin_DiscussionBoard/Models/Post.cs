using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TylerMelvin_DiscussionBoard.Models
{
    public class Post : EntityBase
    {
        public string Title { get; set; }
        public string Content { get; set; }

        [ForeignKey(nameof(DiscussionThread))]
        public int DiscussionThreadId { get; set; }
        public DiscussionThread DiscussionThread { get; set; }

        [ForeignKey(nameof(ParentPost))]
        public int? ParentPostId { get; set; }
        public Post ParentPost { get; set; }

        [NotMapped]
        public List<Post> SubPosts { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public Post()
        {
            CreatedAt = DateTime.Now;
            SubPosts = new List<Post>();
        }
    }
}
