namespace SupportChat.ChatAPI.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public SessionStatus Status { get; set; } = SessionStatus.Pending;
        public DateTime LastPolledAt { get; set; } = DateTime.UtcNow;
        public int MissedPolls { get; set; } = 0;
        public string? AssignedAgentId { get; set; }

    }
}
