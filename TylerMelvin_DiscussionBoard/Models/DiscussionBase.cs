using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TylerMelvin_DiscussionBoard.Models
{
    public abstract class DiscussionBase : EntityBase
    {
        [MaxLength(512)]
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
