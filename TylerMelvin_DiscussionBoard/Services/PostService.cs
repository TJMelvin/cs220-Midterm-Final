using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
using Microsoft.EntityFrameworkCore;

namespace TylerMelvin_DiscussionBoard.Services
{
    public class PostService
    {
        private readonly IRepo<Post> _postRepo;

        public PostService(IRepo<Post> postRepo)
        {
            _postRepo = postRepo;
        }

        public Post? Get(int id)
        {
            return _postRepo.Search(x => x.Id == id).FirstOrDefault();
        }

        public List<Post> GetAll()
        {
            return _postRepo.All().ToList();
        }

        public List<Post> GetForUser(string applicationUserId)
        {
            return _postRepo.Search(x => x.ApplicationUserId == applicationUserId).ToList();
        }

        public List<Post> GetRecycleBin()
        {
            return _postRepo.Search(x => x.IsDeleted).ToList();
        }

        public void Add(Post post)
        {
            _postRepo.Add(post);
            _postRepo.SaveChanges();
        }

        public void Update(Post post)
        {
            _postRepo.Update(post);
            _postRepo.SaveChanges();
        }

        public void Remove(Post post)
        {
            _postRepo.Remove(post);
            _postRepo.SaveChanges();
        }

        public void Delete(Post post)
        {
            post.IsDeleted = true;
            Update(post);
        }

        public void UnDelete(Post post)
        {
            post.IsDeleted = false;
            Update(post);
        }

        public List<Post> GetForThread(int threadId)
        {
            var posts = _postRepo
                .Search(x => !x.IsDeleted && x.DiscussionThreadId == threadId && x.ParentPostId == null)
                .Include(x => x.DiscussionThread)
                .Include(x => x.ApplicationUser)
                .ToList();

            foreach (var post in posts)
            {
                post.SubPosts = LoadHierarchy(post);
            }

            return posts;
        }

        private List<Post> LoadHierarchy(Post post)
        {
            var subposts = _postRepo
                .Search(x => !x.IsDeleted && x.ParentPostId == post.Id)
                .Include(x => x.ParentPost)
                .ToList();

            foreach (var subpost in subposts)
            {
                subpost.SubPosts = LoadHierarchy(subpost);
            }

            return subposts;
        }
    }
}