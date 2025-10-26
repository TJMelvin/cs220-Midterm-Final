using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TylerMelvin_DiscussionBoard.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public List<DiscussionThread> Threads { get; set; }
        public List<Post> Posts { get; set; }

        public ApplicationUser()
        {
            Threads = new List<DiscussionThread>();
            Posts = new List<Post>();
        }
        public override string ToString() => $"{FirstName} {LastName}";
    }
}
