using System.ComponentModel.DataAnnotations;

namespace TylerMelvin_DiscussionBoard.ViewModels
{
    public class ViewItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(512)]
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
