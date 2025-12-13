namespace prisonbreak.Server.DTOs
{
    /// <summary>
    /// DTO pour représenter une conversation de chat.
    /// </summary>
    public class ChatConversationDto
    {
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }
        
        /// <summary>
        /// Items équipés par l'autre utilisateur (avatar, thème, décoration)
        /// </summary>
        public EquippedItemsDto? EquippedItems { get; set; }
    }
}



