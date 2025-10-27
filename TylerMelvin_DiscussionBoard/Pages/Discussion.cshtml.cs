using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;

namespace TylerMelvin_DiscussionBoard.Pages
{
    [Authorize]
    public class DiscussionModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly PostService _postService;
        private readonly ILogger<DiscussionModel> _log;

        [BindProperty(SupportsGet = true)]
        public int ThreadId { get; set; }

        [BindProperty]
        public DiscussionThread DiscussionThread { get; set; }

        public DiscussionModel(DiscussionThreadService threadService, PostService postService, ILogger<DiscussionModel> log)
        {
            _threadService = threadService;
            _postService = postService;
            _log = log;
            DiscussionThread = new DiscussionThread();
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
    }
}