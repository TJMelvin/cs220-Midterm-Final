namespace TylerMelvin_DiscussionBoard.Models
{
    public class EntityBase
    {
        public int id { get; set; }
        public Boolean IsDeleted { get; set; }
        public long Timestamp { get; set; }
    }
}
