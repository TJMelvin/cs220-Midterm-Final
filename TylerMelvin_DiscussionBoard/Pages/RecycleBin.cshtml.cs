using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;
using System.Collections.Generic;
using TylerMelvin_DiscussionBoard.Helpers;

namespace TylerMelvin_DiscussionBoard.Pages
{
    [Authorize(Policy = PolicyTypes.IsAdmin)]
    public class RecycleBinModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly PostService _postService;

        public List<DiscussionThread> DeletedThreads {  get; set; }
        public List<Post> DeletedPosts { get; set; }

        public RecycleBinModel(DiscussionThreadService threadService, PostService postService)
        {
            _threadService = threadService;
            _postService = postService;
        }

        public void OnGet()
        {
            DeletedThreads = _threadService.GetDeletedThreads();
            DeletedPosts = _postService.GetDeletedPosts();
        }

        public IActionResult OnPostRestoreThread(int id)
        {
            var thread = _threadService.Get(id);
            if (thread == null)
            {
                _threadService.UnDelete(thread);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteThread(int id)
        {
            var thread = _threadService.Get(id);
            if (thread != null)
            {
                _threadService.Remove(thread);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRestorePost(int id)
        {
            var post = _postService.Get(id);
            if (post != null)
            {
                _postService.UnDelete(post);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeletePost(int id)
        {
            var post = _postService.Get(id);
            if (post != null)
            {
                _postService.Remove(post);
            }
            return RedirectToPage();
        }
    }
}
