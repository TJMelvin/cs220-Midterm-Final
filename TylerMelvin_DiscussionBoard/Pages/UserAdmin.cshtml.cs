using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Services;

namespace TylerMelvin_DiscussionBoard.Pages
{
    //[Authorize(Policy = PolicyTypes.IsOwnerOrAdmin)]
    public class UserAdminModel : PageModel
    {
        private readonly ApplicationUserService _service;
        private readonly ILogger<UserAdminModel> _logger;

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<Claim> Claims { get; set; }

        //Constructor
        public UserAdminModel(ApplicationUserService service, ILogger<UserAdminModel> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Users = new List<ApplicationUser>();
            Claims = new List<Claim>();
            ApplicationUser = new ApplicationUser();
        }

        public async Task OnGetAsync()
        {
            try
            {
                Users = _service.GetAllUsers() ?? new List<ApplicationUser>();

                if (!string.IsNullOrEmpty(Id))
                {
                    ApplicationUser = await _service.GetUserAsync(Id);
                    Claims = await _service.GetApplicationClaimsAsync(Id);
                    IsAdmin = await _service.IsAdminAsync(Id);
                    IsModerator = await _service.IsModeratorAsync(Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading UserAdmin page for Id={Id}", Id);
                // Keep default values if something goes wrong
                Users = Users ?? new List<ApplicationUser>();
                Claims = Claims ?? new List<Claim>();
                ApplicationUser = ApplicationUser ?? new ApplicationUser();
            }
        }
        public async Task<IActionResult> OnPostAsync(string Type, string Value)
        {
            try
            {
                await _service.UpsertUserClaimsAsync(Id, Type, Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user claim for user {UserId}", Id);
            }

            // Refresh the page and keep the user selected
            return LocalRedirect($"/UserAdmin?Id={Id}");
        }
    }
}