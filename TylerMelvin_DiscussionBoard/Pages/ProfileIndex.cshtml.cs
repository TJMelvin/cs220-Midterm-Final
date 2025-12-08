using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TylerMelvin_DiscussionBoard.Models;
using System;
using System.Threading.Tasks;

namespace TylerMelvin_DiscussionBoard.Pages
{
    public class ProfileIndexModel : PageModel
    {
        public readonly UserManager<ApplicationUser> _userManager;

        public ProfileIndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public ApplicationUser User { get; set; }
        public string AvatarBase64 { get; set; }
        public string Initial {  get; set; }


        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null)
                return NotFound();

            User = await _userManager.FindByIdAsync(id);

            if (User == null)
                return NotFound();

            if (User.AvatarImage != null)
                AvatarBase64 = "data:image/png;base64," + Convert.ToBase64String(User.AvatarImage);
                
            Initial = User.Email.Substring(0, 1).ToUpper();

            return Page();
        }
    }
}
