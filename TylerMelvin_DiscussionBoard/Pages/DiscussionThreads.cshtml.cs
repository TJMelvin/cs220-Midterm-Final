using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public DiscussionThread DiscussionThread { get; set; }

        [BindProperty]
        public ViewItem Discussion { get; set; }

        public DiscussionThreadsModel(DiscussionThreadService service, ILogger<DiscussionThreadsModel> log)
        {
            _service = service;
            _log = log;

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
                        Content = DiscussionThread.Content,
                        CreatedAt = DiscussionThread.CreatedAt
                    };
                    _log.LogInformation($"Fetched {DiscussionThread.Id} threads");
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

                DiscussionThread savedThread;

                if (Discussion.Id != 0)
                {
                    var existingThread = _service.Get(Discussion.Id);
                    if (existingThread == null)
                    {
                        _log.LogWarning($"No thread found with ID {Discussion.Id}");
                        return NotFound();
                    }

                    existingThread.Title = Discussion.Title;
                    existingThread.Content = Discussion.Content;
                    _service.Update(existingThread);
                    savedThread = existingThread;

                    _log.LogInformation($"Updated DiscussionThread with ID {Discussion.Id}");
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        _log.LogWarning("No user ID found; redirecting to login.");
                        return RedirectToPage("/Account/Login");
                    }

                    var newThread = new DiscussionThread
                    {
                        Title = Discussion.Title,
                        Content = Discussion.Content,
                        ApplicationUserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    savedThread = _service.Add(newThread);
                    _log.LogInformation($"Added new DiscussionThread with ID {savedThread.Id}");
                }

                return RedirectToPage("/DiscussionThreads", new { id = savedThread.Id });
            }
            catch (Exception ex)
            {
                _log.LogError($"Error in OnPost: {ex.Message}");
                return Page();
            }
        }
    }
}
