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
        private readonly DiscussionThreadService _threadService;
        private readonly PostService _postService;
        private readonly ILogger<ReplyModel> _log;
        
        [BindProperty(SupportsGet = true)]
        public int ThreadId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PostId { get; set; }

        [BindProperty(SupportsGet = true)]
        public ViewItem Parent { get; set; }

        [BindProperty]
        public Post Post { get; set; }

        public ReplyModel(DiscussionThreadService threadService, PostService postService, ILogger<ReplyModel> log)
        {
            _threadService = threadService;
            _postService = postService;
            _log = log;

            Parent = new ViewItem();
            Post = new Post();
        }

        public void OnGet()
        {
            try
            {
                if (ThreadId > 0)
                {
                    var thread = _threadService.Get(ThreadId);
                    Parent.Id = thread.Id;
                    Parent.Title = thread.Title;
                    Parent.Content = thread.Content;
                }
                if (PostId.HasValue && PostId.Value > 0)
                {
                    var parentPost = _postService.Get(PostId.Value);
                    Parent.Id = parentPost.Id;
                    Parent.Title = parentPost.Title;
                    Parent.Content = parentPost.Content;
                }
                _log.LogInformation($"Loaded DiscussionThread with ID {ThreadId} successfully.");
            }
            catch (Exception ex)
            {
                _log.LogError($"Error loading DiscussionThread {ThreadId}: {ex.Message}");
            }
        }

        [Authorize]
        public IActionResult OnPost()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _log.LogError("Model state invalid in Reply OnPost");
                    return Page();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _log.LogWarning("User ID missing when posting reply");
                    return RedirectToPage("/Account/Login");
                }

                // Map view model to Post entity
                var newPost = new Post
                {
                    Title = Post.Title,
                    Content = Post.Content,
                    DiscussionThreadId = ThreadId,
                    ParentPostId = PostId.HasValue && PostId.Value > 0 ? (int?)PostId.Value : null,
                    ApplicationUser = null,
                    ApplicationUserId = userId
                };

                var saved = _postService.Add(newPost);
                _log.LogInformation("Added reply with Id {PostId}", saved.Id);

                return RedirectToPage("/Discussion", new { ThreadId = ThreadId });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in Reply OnPost for ThreadId {ThreadId}", ThreadId);
                return Page();
            }
        }
    }
}