using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TylerMelvin_DiscussionBoard.Models
{
    public abstract class EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }

        [ConcurrencyCheck]
        public long Timestamp { get; set; }
    }
}
