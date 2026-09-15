namespace SFM_BE.Entities.Interface
{
    public interface ISoftDeletable
    {
        public DateTime? DeletedAt { get; set; }
    }
}
