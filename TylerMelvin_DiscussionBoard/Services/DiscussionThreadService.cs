using Microsoft.EntityFrameworkCore;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
namespace TylerMelvin_DiscussionBoard.Services 
{ 
    public class DiscussionThreadService 
    {
        private readonly IRepo<DiscussionThread> _repo;
        private readonly IRepo<Post> _postRepo;  
        private readonly PostService _postService;
        private readonly ILogger<DiscussionThreadService> _log;
        private readonly IConfiguration _config;

        public DiscussionThreadService(
            IRepo<DiscussionThread> repo,
            IRepo<Post> postRepo,
            PostService postService,
            IConfiguration config,
            ILogger<DiscussionThreadService> log) 
        { 
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _postRepo = postRepo ?? throw new ArgumentNullException(nameof(postRepo));
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _config = config;
            _log = log;
        } 

        //get for one
        public DiscussionThread Get(int Id) 
        { 
            var dt = new DiscussionThread();
            try 
                { 
                dt = _repo 
                .Search(x => x.Id == Id && !x.IsDeleted) 
                .Include(x => x.ApplicationUser)
                .FirstOrDefault() ?? new DiscussionThread();

                dt.Posts = _postService.GetForThread(Id);
                dt.PostCount = dt.Posts?.Count ?? 0;
            } 
            catch (Exception ex) 
            { 
                _log.LogError($"Error getting Discussion Thread with {Id}. " + ex.Message, Id); 
            } 
            return dt; 
        } 

        //get list for all
        public List<DiscussionThread> GetAll() 
        { 
            try 
            { 
                var threads = _repo
                    .All() 
                    .Include(x => x.ApplicationUser) 
                    .Include(x => x.Posts) 
                    .Where(x => !x.IsDeleted) 
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();

                foreach (var t in threads)
                {
                    t.Posts = _postService.GetForThread(t.Id);
                    t.PostCount = t.Posts?.Count ?? 0;
                }
                return threads;
            } 
            catch (Exception ex) 
            { 
                _log.LogError("Error getting all discussion threads. " + ex.Message); 
                return new List<DiscussionThread>();
            }  
        } 

        //Get list for user
        public List<DiscussionThread> GetForUser(string ApplicationUserId) 
        {
            try
            {
                var threads = _repo
                    .Search(x => x.ApplicationUserId == ApplicationUserId && !x.IsDeleted)
                    .Include(x => x.ApplicationUser)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();

                foreach (var t in threads)
                {
                    t.Posts = _postService.GetForThread(t.Id);
                    t.PostCount = t.Posts?.Count ?? 0;
                }
                return threads;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error getting threads for user {UserId}", ApplicationUserId);
                return new List<DiscussionThread>();
            }
        } 

        //get trash
        public List<DiscussionThread> GetRecycleBin() 
        {
            try
            {
                var threads = _repo.Search(x => x.IsDeleted)
                                   .Include(x => x.ApplicationUser)
                                   .ToList();

                foreach (var t in threads)
                {
                    t.Posts = _postService.GetForThread(t.Id);
                    t.PostCount = t.Posts?.Count ?? 0;
                }

                return threads;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error getting recycle bin threads");
                return new List<DiscussionThread>();
            }
        } 
        
        //add
        public DiscussionThread Add(DiscussionThread thread) 
        {
            if (thread == null) throw new ArgumentNullException(nameof(thread));
            try
            {
                _repo.Add(thread);
                _repo.SaveChanges();
                _log.LogInformation("Added DiscussionThread {ThreadId}", thread.Id);

                thread.Posts = new List<Post>();
                thread.PostCount = 0;

                return thread;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error adding DiscussionThread");
                throw;
            }
        } 
        
        //update
        public void Update(DiscussionThread thread) 
        {
            if (thread == null) throw new ArgumentNullException(nameof(thread));
            try
            {
                _repo.Update(thread);
                _repo.SaveChanges();
                _log.LogInformation("Updated DiscussionThread {ThreadId}", thread.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _log.LogWarning(ex, "Concurrency exception updating thread {ThreadId}", thread.Id);
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating DiscussionThread {ThreadId}", thread.Id);
                throw;
            }
        } 
        
        //remove
          public void Remove(DiscussionThread thread) 
        {
            if (thread == null) throw new ArgumentNullException(nameof(thread));
            try
            {
                _repo.Remove(thread);
                _repo.SaveChanges();
                _log.LogInformation("Removed DiscussionThread {ThreadId}", thread.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error removing DiscussionThread {ThreadId}", thread.Id);
                throw;
            }
        } 
        
        //delete
        public void Delete(DiscussionThread thread) 
        {
            if (thread == null) throw new ArgumentNullException(nameof(thread));
            try
            {
                thread.IsDeleted = true;
                _repo.Update(thread);
                _repo.SaveChanges();
                _log.LogInformation("Soft-deleted DiscussionThread {ThreadId}", thread.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error soft-deleting DiscussionThread {ThreadId}", thread.Id);
                throw;
            }
        } 
        
        //undelete
        public void UnDelete(DiscussionThread thread) 
        {
            if(thread == null) throw new ArgumentNullException(nameof(thread));
            try
            {
                thread.IsDeleted = false;
                _repo.Update(thread);
                _repo.SaveChanges();
                _log.LogInformation("Restored DiscussionThread {ThreadId}", thread.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error undeleting DiscussionThread {ThreadId}", thread.Id);
                throw;
            }
        } 

        //count posts
        public int CountPosts(int threadId)
        {
            try
            {
                return _postRepo.Search(x => !x.IsDeleted && x.DiscussionThreadId == threadId).Count();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error counting posts for thread {ThreadId}", threadId);
                return 0;
            }
        }
    } 
}