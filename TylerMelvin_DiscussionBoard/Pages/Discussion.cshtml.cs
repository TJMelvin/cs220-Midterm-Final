using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;

namespace TylerMelvin_DiscussionBoard.Pages
{
    public class DiscussionModel : PageModel
    {
        private readonly ILogger<DiscussionModel> _log;
        private readonly DiscussionThreadService _service;
        [BindProperty(SupportsGet = true)]
        public int ThreadId { get; set; }

        [BindProperty]
        public DiscussionThread DiscussionThread { get; set; } = new DiscussionThread();

        public DiscussionModel(ILogger<DiscussionModel> log, DiscussionThreadService service)
        {
            _log = log;
            _service = service;
        }
        public void OnGet()
        {
            try
            {
                DiscussionThread = _service.Get(ThreadId);
            }
            catch (Exception ex)
            {
                _log.LogError($"Failed to load posts for ThreadId {ThreadId}: {ex.Message}");
            }
        }
    }
}
