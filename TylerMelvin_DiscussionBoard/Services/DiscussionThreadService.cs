using Microsoft.EntityFrameworkCore;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
namespace TylerMelvin_DiscussionBoard.Services 
{ 
    public class DiscussionThreadService 
    { 
        private readonly IRepo<DiscussionThread> _repo; 
        private readonly PostService _postService;

        private readonly ILogger _log; 

        private DiscussionThread _dt; 
        private List<DiscussionThread> _dts; 
        public DiscussionThreadService(
            IRepo<DiscussionThread> repo, 
            PostService postService, 
            ILogger<DiscussionThreadService> log) 
        { 
            _repo = repo;
            _log = log; 
            _postService = postService;
        } 

        //get for one
        public DiscussionThread Get(int Id) 
        { 
            DiscussionThread thread = new DiscussionThread();
            try 
                { 
                thread = _repo 
                .Search(x => x.Id == Id && !x.IsDeleted) 
                .Include(x => x.ApplicationUser)
                .FirstOrDefault() ?? new DiscussionThread();

                thread.Posts = _postService.GetForThread(Id);
            } 
            catch (Exception ex) 
            { 
                _log.LogError($"Error getting Discussion Thread with {Id}. " + ex.Message, Id); 
            } 
            return thread; 
        } 
        //get list for all
        public List<DiscussionThread> GetAll() 
        { 
            List<DiscussionThread> threads = new List<DiscussionThread>();
            try 
            { threads = _repo.All() 
                    .Include(x => x.ApplicationUser) 
                    .Include(x => x.Posts) 
                    .Where(x => !x.IsDeleted) 
                    .ToList(); 
            } 
            catch (Exception ex) 
            { 
                _log.LogError("Error getting all discussion threads. " + ex.Message); 
                _dts = new List<DiscussionThread>(); 
            } 
            return threads; 
        } 
        //Get list for user
        public List<DiscussionThread> GetForUser(string ApplicationUserId) 
        { 
            return _repo.Search(x => x.ApplicationUserId == ApplicationUserId && !x.IsDeleted) 
                .Include(x => x.Posts) 
                .Include(x => x.ApplicationUser) 
                .ToList(); 
        } 
        //get trash
        public List<DiscussionThread> GetRecycleBin() 
        { 
            return _repo.Search(x => x.IsDeleted) 
                .Include(x => x.ApplicationUser) 
                .ToList(); 
        } 
        
        //add
        public DiscussionThread Add(DiscussionThread thread) 
        { 
            _repo.Add(thread);
            _repo.SaveChanges();
            return thread;
        } 
        
        //update
        public void Update(DiscussionThread thread) 
        { 
            _repo.Update(thread); 
            _repo.SaveChanges(); 
        } 
        
        //remove
          public void Remove(DiscussionThread thread) 
        { _repo.Remove(thread); 
            _repo.SaveChanges(); 
        } 
        
        //delete
        public void Delete(DiscussionThread thread) 
        { 
            thread.IsDeleted = true; 
            _repo.Update(thread); 
            _repo.SaveChanges(); 
        } 
        
        //undelete
        public void UnDelete(DiscussionThread thread) 
        { thread.IsDeleted = false; 
            _repo.Update(thread); 
            _repo.SaveChanges(); 
        } 

        //count posts
        public int CountPosts(int threadId)
        {
            var posts = _postService.GetForThread(threadId);
            return posts.Count;
        }
    } 
}