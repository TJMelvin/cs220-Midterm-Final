using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;

namespace TylerMelvin_DiscussionBoard.Pages
{
    [Authorize]
    public class DiscussionModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly DiscussionThreadService _threadService;
        private readonly PostService _postService;
        private readonly ILogger<DiscussionModel> _log;

        [BindProperty(SupportsGet = true)]
        public int ThreadId { get; set; }

        [BindProperty]
        public DiscussionThread DiscussionThread { get; set; }

        public DiscussionModel(
            DiscussionThreadService threadService,
            PostService postService,
            ILogger<DiscussionModel> log,
            IAuthorizationService authorizationService)
        {
            _threadService = threadService;
            _postService = postService;
            _log = log;
            _authorizationService = authorizationService;
        }

        public void OnGet()
        {
            try
            {
                DiscussionThread = _threadService.Get(ThreadId);
                ViewData["Title"] = "Discussion Post - " + DiscussionThread.Title;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error loading Discussion thread {ThreadId}", ThreadId);
                DiscussionThread = new DiscussionThread();
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                var thread = _threadService.Get(ThreadId);

                if (thread == null)
                {
                    return NotFound();
                }

                // Check authorization
                var isAuthorized = await _authorizationService.AuthorizeAsync(
                    User, thread, PolicyTypes.IsOwnerOrAdmin);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                // Soft delete
                _threadService.Delete(thread);

                return RedirectToPage("/Index");  // Or your main thread list page
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting discussion thread {ThreadId}", ThreadId);
                return Page();
            }
        }
    }
}