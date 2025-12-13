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
        
        /// <summary>
        /// Items équipés par l'ami (avatar, thème, décoration)
        /// </summary>
        public EquippedItemsDto? EquippedItems { get; set; }
    }

    /// <summary>
    /// DTO pour les items équipés d'un utilisateur
    /// </summary>
    public class EquippedItemsDto
    {
        public ItemDto? Avatar { get; set; }
        public ItemDto? Theme { get; set; }
        public ItemDto? Decoration { get; set; }
    }
}



