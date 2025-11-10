using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;

namespace TylerMelvin_DiscussionBoard.Authorization
{
    public class IsOwnerOrAdminHandler : AuthorizationHandler<IsOwnerOrAdminRequirement, DiscussionBase>
    {
        public IsOwnerOrAdminHandler()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                IsOwnerOrAdminRequirement requirement,
                DiscussionBase resource)
        {
            if (context.User.HasClaim(c => c.Type == PolicyTypes.IsAdmin && c.Value == PolicyValues.True))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && resource.ApplicationUserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
