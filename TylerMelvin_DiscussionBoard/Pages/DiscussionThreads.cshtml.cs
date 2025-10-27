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
    public class DiscussionThreadsModel : PageModel
    {
        private readonly DiscussionThreadService _service;
        private readonly ILogger<DiscussionThreadsModel> _log;
        [BindProperty]
        public DiscussionThread DiscussionThread { get; set; }
        
        [BindProperty]
        public ViewItem Discussion { get; set; }

        public DiscussionThreadsModel(DiscussionThreadService service, ILogger<DiscussionThreadsModel> log)
        {
            _service = service;
            _log = log;

            DiscussionThread = new DiscussionThread();
            Discussion = new ViewItem();
        }

        public void OnGet(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    DiscussionThread = _service.Get(id.Value);
                    Discussion = new ViewItem
                    {
                        Id = DiscussionThread.Id,
                        Title = DiscussionThread.Title,
                        Content = DiscussionThread.Content
                    };
                    _log.LogInformation($"Loaded DiscussionThread with ID {id.Value}");
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning($"Failed to load DiscussionThread: {ex.Message}");
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    _log.LogError("ModelState is invalid.");
                    return Page();
                }

                DiscussionThread savedThread;
                if (Discussion.Id != 0)
                {
                    var existingThread = _service.Get(Discussion.Id);
                    existingThread.Title = Discussion.Title;
                    existingThread.Content = Discussion.Content;
                    _service.Update(existingThread);
                    _log.LogInformation($"Updated DiscussionThread with ID {Discussion.Id}");
                    return LocalRedirect("/DiscussionThreads/" + existingThread.Id);
                    savedThread = existingThread;
                }
                else
                {
                    DiscussionThread.Title = Discussion.Title;
                    DiscussionThread.Content = Discussion.Content;
                    DiscussionThread.ApplicationUser = null;

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        _log.LogWarning("No user ID found; redirecting to login.");
                        return RedirectToPage("/Account/Login");
                    }

                    DiscussionThread.ApplicationUserId = userId;
                    savedThread = _service.Add(DiscussionThread);
                    _log.LogInformation($"Added new DiscussionThread with ID {savedThread.Id}");
                }
                return RedirectToPage("/DiscussionThreads", new { id = savedThread.Id });
            }
            catch (Exception ex)
            {
                _log.LogError($"Error in onpost: {ex.Message}");
                return Page();
            }
        }
    }
}
