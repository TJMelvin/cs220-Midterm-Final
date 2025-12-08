using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;
using TylerMelvin_DiscussionBoard.Helpers;

namespace TylerMelvin_DiscussionBoard.Pages
{
    [Authorize]
    public class EditThreadModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly IAuthorizationService _authorization;

        public EditThreadModel(
            DiscussionThreadService threadService,
            IAuthorizationService authorization)
        {
            _threadService = threadService;
            _authorization = authorization;
        }

        [BindProperty]
        public DiscussionThread Thread { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Thread = _threadService.Get(id);

            if (Thread == null)
                return NotFound();

            var auth = await _authorization.AuthorizeAsync(User, Thread, PolicyTypes.IsOwnerOrAdmin);
            if (!auth.Succeeded)
                return LocalRedirect("/Identity/Account/AccessDenied");

            if (!string.IsNullOrWhiteSpace(Thread.Content))
            {
                var trimmed = Thread.Content.Trim();

                if (trimmed.StartsWith("<p>") && trimmed.EndsWith("</p>"))
                {
                    Thread.Content = trimmed.Substring(3, trimmed.Length - 7);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = _threadService.Get(Thread.Id);

            if (existing == null)
                return NotFound();

            var auth = await _authorization.AuthorizeAsync(User, existing, PolicyTypes.IsOwnerOrAdmin);
            if (!auth.Succeeded)
                return LocalRedirect("/Identity/Account/AccessDenied");

            existing.Title = Thread.Title;
            existing.Content = Thread.Content;

            _threadService.Update(existing);

            return RedirectToPage("/Discussion", new { threadId = existing.Id });
        }
    }
}