using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TylerMelvin_DiscussionBoard.Models
{
    public class DiscussionThread : DiscussionBase
    {
        public List<Post> Posts { get; set; }

        [NotMapped]
        public int PostCount { get; set; }

        public DiscussionThread() 
        {
            Posts = new List<Post>();
            CreatedAt = DateTime.Now;
            PostCount = 0;
        }
    }
}
