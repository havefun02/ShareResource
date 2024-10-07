namespace ShareResource.Models.Entities
{
    public class Token
    {
        public int TokenId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiredAt {  get; set; }
        public bool IsRevoked { get; set; }

        public string? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
