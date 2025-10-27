using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;

namespace TylerMelvin_DiscussionBoard.Services
{
    public class PostService
    {
        private readonly IRepo<Post> _postRepo;
        private readonly IConfiguration _config;
        private readonly ILogger<PostService> _log;

        public PostService(IRepo<Post> postRepo, IConfiguration config, ILogger<PostService> log)
        {
            _postRepo = postRepo;
            _config = config;
            _log = log;
        }

        public List<Post> GetForThread(int threadId)
        {
            try
            {
                var posts = _postRepo
                    .Search(x => !x.IsDeleted && x.DiscussionThreadId == threadId && x.ParentPostId == null)
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.DiscussionThread)
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                foreach (var p in posts)
                {
                    p.SubPosts = LoadHeirarchy(p);
                }
                return posts;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error loading posts for thread {threadId}", threadId);
                return new List<Post>();
            }
        }

        private List<Post> LoadHeirarchy(Post post)
        {
            var subposts = new List<Post>();

            try
            {
                subposts = _postRepo
                    .Search(x => !x.IsDeleted && x.ParentPostId == post.Id)
                    .Include(x => x.ParentPost)
                    .Include(x => x.ApplicationUser)
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                foreach (var p in subposts)
                {
                    p.SubPosts = LoadHeirarchy(p);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error loading subposts for PostId {PostId}", post?.Id); ;
            }
            return subposts;
        }

        public Post Add(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            try
            {
                _postRepo.Add(post);
                _postRepo.SaveChanges();
                _log.LogInformation("Added Post with ID {PostId}", post.Id);
                return post;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error adding post");
                throw;
            }
        }

        public void Update(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            try
            {
                _postRepo.Update(post);
                _postRepo.SaveChanges();
                _log.LogInformation("Updated Post with ID {PostId}", post.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating post {PostId}", post?.Id);
                throw;
            }
        }

        public void Remove(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            try
            {
                _postRepo.Remove(post);
                _postRepo.SaveChanges();
                _log.LogInformation("Removed Post with ID {PostId}", post.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error removing post {PostId}", post?.Id);
                throw;
            }
        }

        public void Delete(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            try
            {
                post.IsDeleted = true;
                _postRepo.Update(post);
                _postRepo.SaveChanges();
                _log.LogInformation("Soft-deleted Post with ID {PostId}", post.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error soft deleting post {PostId}", post?.Id);
                throw;
            }
        }

        public void UnDelete(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            try
            {
                post.IsDeleted = false;
                _postRepo.Update(post);
                _postRepo.SaveChanges();
                _log.LogInformation("Undeleted Post with ID {PostId}", post.Id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error undeleting post {PostId}", post?.Id);
                throw;
            }
        }

        public Post Get(int id)
        {
            try
            {
                var p = _postRepo
                    .Search(x => x.Id == id && !x.IsDeleted)
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.ParentPost)
                    .Include(x => x.DiscussionThread)
                    .FirstOrDefault();

                return p ?? new Post();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error getting post {PostId}", id);
                return new Post();
            }
        }

        public int CountForThread(int threadId)
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