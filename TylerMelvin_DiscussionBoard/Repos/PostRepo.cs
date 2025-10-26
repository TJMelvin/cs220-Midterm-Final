using TylerMelvin_DiscussionBoard.Data;
using TylerMelvin_DiscussionBoard.Models;

namespace TylerMelvin_DiscussionBoard.Repos
{
    public class PostRepo : RepoBase<Post>
    {
        public PostRepo(ApplicationDbContext context) : base(context) 
        { 
        }
    }
}
