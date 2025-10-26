using TylerMelvin_DiscussionBoard.Data;
using TylerMelvin_DiscussionBoard.Models;

namespace TylerMelvin_DiscussionBoard.Repos
{
    public class DiscussionThreadRepo : RepoBase<DiscussionThread>
    {
        public DiscussionThreadRepo(ApplicationDbContext context) : base(context) 
        { 
        }
    }
}
