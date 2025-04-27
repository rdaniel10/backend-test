namespace SupportChat.ChatAPI.Models
{
    public class Agent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AgentSeniority Seniority { get; set; }

        public List<string> AssignedSessionIds { get; set; } = new();

        public int CurrentLoad => AssignedSessionIds.Count;

        public double Efficiency => Seniority switch
        {
            AgentSeniority.Junior => 0.4,
            AgentSeniority.Mid => 0.6,
            AgentSeniority.Senior => 0.8,
            AgentSeniority.TeamLead => 0.5,
            _ => 0.4
        };

        public int MaxConcurrency => (int)(10 * Efficiency);

        public bool IsAvailable => CurrentLoad < MaxConcurrency;

        public bool IsOnShift { get; set; } = true;
    }

    public enum AgentSeniority
    {
        Junior,
        Mid,
        Senior,
        TeamLead
    }
}
