using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;
using TylerMelvin_DiscussionBoard.ViewModels;

namespace TylerMelvin_DiscussionBoard.Pages
{
    [Authorize]
    public class ReplyModel : PageModel
    {
        private readonly DiscussionThreadService _discussionService;
        private readonly PostService _postService;
        private readonly ILogger<ReplyModel> _log;

        public DiscussionThread DiscussionThread { get; set; }
        public int ThreadId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PostId { get; set; }

        [BindProperty(SupportsGet = true)]
        public ViewItem Parent { get; set; }

        [BindProperty]
        private Post Post { get; set; }

        public ReplyModel(
            DiscussionThreadService discussionService,
            PostService postService,
            ILogger<ReplyModel> log)
        {
            _discussionService = discussionService;
            _postService = postService;
            _log = log;

            DiscussionThread = new DiscussionThread();
            Parent = new ViewItem();
            Post = new Post();
        }

        public void OnGet()
        {
            try
            {
                if (ThreadId > 0)
                {
                    // Load discussion thread
                    DiscussionThread = _discussionService.Get(ThreadId);
                    Parent.Id = DiscussionThread.Id;
                    Parent.Title = DiscussionThread.Title;
                    Parent.Content = DiscussionThread.Content;

                    // If replying to a specific post
                    if (PostId.HasValue && PostId.Value > 0)
                    {
                        var parentPost = _postService.Get(PostId.Value);
                        if (parentPost != null)
                        {
                            Parent.Id = parentPost.Id;
                            Parent.Title = parentPost.Title;
                            Parent.Content = parentPost.Content;
                        }
                    }

                    _log.LogInformation($"Loaded DiscussionThread with ID {ThreadId} successfully.");
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error loading DiscussionThread {ThreadId}: {ex.Message}");
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                if (PostId.HasValue && PostId.Value > 0)
                {
                    Post.ParentPostId = PostId.Value;
                }
                Post.DiscussionThreadId = ThreadId;
                Post.Title = Parent.Title;
                Post.Content = Parent.Content;
                Post.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                Post.DiscussionThread = null;
                Post.ApplicationUser = null;
                Post.ParentPost = null;

                _postService.Add(Post);
                _log.LogInformation("Added reply with Id {PostId}", Post.Id);
            }
            catch (Exception ex)
            {
                _log.LogError($"Error adding reply for ThreadId {ThreadId}: {ex.Message}");
            }

            return LocalRedirect("/Discussion/" + ThreadId);
        }
    }
}