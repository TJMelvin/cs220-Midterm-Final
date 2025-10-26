using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TylerMelvin_DiscussionBoard.Models
{
    public class Post : DiscussionBase
    {
        [ForeignKey(nameof(DiscussionThread))]
        public int DiscussionThreadId {  get; set; }

        public DiscussionThread DiscussionThread { get; set; }

        [ForeignKey(nameof(ParentPost))]
        public int? ParentPostId { get; set; }
        public Post ParentPost { get; set; }

        public List<Post> SubPosts { get; set; }

        public Post()
        {
            SubPosts = new List<Post>();
            CreatedAt = DateTime.Now;
        }
    }
}
