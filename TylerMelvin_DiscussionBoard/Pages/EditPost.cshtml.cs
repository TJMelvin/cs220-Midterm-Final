using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;

namespace TylerMelvin_DiscussionBoard.Pages
{
    //[Authorize]
    public class EditPostModel : PageModel
    {
        private readonly PostService _postService;
        private readonly IAuthorizationService _authorization;

        public EditPostModel(PostService postService, IAuthorizationService authorization)
        {
            _postService = postService;
            _authorization = authorization;
        }

        [BindProperty]
        public Post Post { get; set; }

        public int ThreadId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = _postService.Get(id);
            if (Post == null || Post.Id == 0)
                return NotFound();

            ThreadId = Post.DiscussionThreadId;

            //var auth = await _authorization.AuthorizeAsync(User, Post, PolicyTypes.IsOwnerOrAdmin);
            //if (!auth.Succeeded)
               // return LocalRedirect("/Identity/Account/AccessDenied");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = _postService.Get(Post.Id);
            if (existing == null || existing.Id == 0)
                return NotFound();

            //var auth = await _authorization.AuthorizeAsync(User, existing, PolicyTypes.IsOwnerOrAdmin);
            //if (!auth.Succeeded)
                //return LocalRedirect("/Identity/Account/AccessDenied");

            existing.Title = Post.Title;
            existing.Content = Post.Content;

            _postService.Update(existing);

            return RedirectToPage("/Discussion", new { ThreadId = existing.DiscussionThreadId });
        }
    }
}