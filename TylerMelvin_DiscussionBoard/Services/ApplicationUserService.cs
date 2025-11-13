using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;

namespace TylerMelvin_DiscussionBoard.Services
{
    public class ApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ApplicationUserService> _logger;
        private ApplicationUser ApplicationUser;
        private List<Claim> Claims;

        //Constructor
        public ApplicationUserService(UserManager<ApplicationUser> userManager, ILogger<ApplicationUserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
            ApplicationUser = new ApplicationUser();
            Claims = new List<Claim>();
        }

        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException($"No user with the UserId {userId} value was found.");
            }

            ApplicationUser = user;
            return user;
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }

        public async Task<bool> IsAdminAsync(string userId)
        {
            var claims = await GetApplicationClaimsAsync(userId);
            return claims.Any(c => c.Type == PolicyTypes.IsAdmin && c.Value == PolicyValues.True);
        }

        public async Task<bool> IsModeratorAsync(string userId)
        {
            var claims = await GetApplicationClaimsAsync(userId);
            return claims.Any(c => c.Type == PolicyTypes.IsModerator && c.Value == PolicyValues.True);
        }

        public async Task<List<Claim>> GetApplicationClaimsAsync(string userId)
        {
            if (ApplicationUser == null || !ApplicationUser.Id.Equals(userId))
            {
                ApplicationUser = await GetUserAsync(userId);
            }

            Claims = (List<Claim>)await _userManager.GetClaimsAsync(ApplicationUser);
            return Claims;
        }

        public async Task UpsertUserClaimsAsync(string userId, string type, string value)
        {
            Claim? claim = null;

            try
            {
                // Validate type and create a claim if valid
                if (type.Equals(PolicyTypes.IsAdmin) || type.Equals(PolicyTypes.IsModerator))
                {
                    claim = new Claim(type, value);
                }
                else
                {
                    throw new InvalidOperationException($"{type} is not a valid claim for this application.");
                }

                // Ensure valid input
                if (!string.IsNullOrEmpty(userId) && claim != null)
                {
                    if (ApplicationUser == null || !ApplicationUser.Id.Equals(userId))
                    {
                        ApplicationUser = await GetUserAsync(userId);
                    }

                    // Remove existing claim of the same type before adding
                    foreach (Claim c in await GetApplicationClaimsAsync(userId))
                    {
                        if (c.Type.Equals(claim.Type))
                        {
                            await _userManager.RemoveClaimAsync(ApplicationUser, c);
                            break;
                        }
                    }

                    // Add new claim
                    var result = await _userManager.AddClaimAsync(ApplicationUser, claim);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Added claim {claim.Type} with value {claim.Value} to user {userId}");
                    }
                    else
                    {
                        var msg = $"Failed to add claim {claim.Type} with value {claim.Value} to user {userId}";
                        _logger.LogError(msg);
                        throw new InvalidOperationException(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpsertUserClaimsAsync: {ex.Message}");
            }
        }
    }
}
