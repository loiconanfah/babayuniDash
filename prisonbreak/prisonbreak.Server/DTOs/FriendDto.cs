namespace prisonbreak.Server.DTOs
{
    /// <summary>
    /// DTO pour représenter un ami.
    /// </summary>
    public class FriendDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLoginAt { get; set; }
        public bool IsOnline { get; set; }
        public DateTime FriendsSince { get; set; }
    }
}



