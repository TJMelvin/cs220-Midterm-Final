using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;

namespace TylerMelvin_DiscussionBoard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _log;
        private readonly DiscussionThreadService _discussionService;

        public List<DiscussionThread> DiscussionThreads { get; set; }

        public IndexModel(DiscussionThreadService discussionService, ILogger<IndexModel> log)
        {
            _discussionService = discussionService;
            _log = log;
            DiscussionThreads = new List<DiscussionThread>();
        }

        public void OnGet()
        {
            try
            {
                DiscussionThreads = _discussionService.GetAll();
                _log.LogInformation($"Loaded {DiscussionThreads.Count} discussion threads.");
            }
            catch (Exception ex)
            {
                _log.LogError($"Could not load discussion threads: {ex.Message}");
            }
        }
    }
}
