namespace SupportChat.ChatAPI.Models
{
    public class AgentTeam
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; } = new();
    }
}
