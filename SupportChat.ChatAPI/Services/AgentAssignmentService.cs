using SupportChat.ChatAPI.Models;

namespace SupportChat.ChatAPI.Services
{
    public class AgentAssignmentService
    {
        private readonly List<AgentTeam> _teams;
        private int _lastAssignedIndex = -1;

        public AgentAssignmentService(List<AgentTeam> teams)
        {
            _teams = teams;
        }

        public List<Agent> GetAllAgents()
        {
            return _teams.SelectMany(t => t.Agents).ToList();
        }

        public Agent? GetNextAvailableAgent()
        {
            var allAgents = _teams
                .SelectMany(t => t.Agents)
                .Where(a => a.IsOnShift && a.IsAvailable)
                .OrderBy(a => a.Seniority) // This is to prefer lower seniority
                .ToList();

            if (!allAgents.Any()) return null;

            _lastAssignedIndex = (_lastAssignedIndex + 1) % allAgents.Count;
            return allAgents[_lastAssignedIndex];
        }

        public bool AssignChat(ChatSession session)
        {
            var agent = GetNextAvailableAgent();
            if (agent == null) return false;

            session.AssignedAgentId = agent.Id;
            session.Status = SessionStatus.Assigned;

            agent.AssignedSessionIds.Add(session.Id.ToString());

            return true;
        }

        public Agent? AssignAgent(ChatSession session)
        {
            foreach (var team in _teams)
            {
                foreach (var seniorityGroup in Enum.GetValues<AgentSeniority>())
                {
                    var agents = team.Agents
                        .Where(a => a.Seniority == seniorityGroup && a.IsAvailable)
                        .OrderBy(_ => Guid.NewGuid())
                        .ToList();

                    if (agents.Any())
                    {
                        var agent = agents.First();
                        session.AssignedAgentId = agent.Id;
                        session.Status = SessionStatus.Assigned;
                        agent.AssignedSessionIds.Add(session.Id.ToString());
                        return agent;
                    }
                }
            }

            return null; // No one available
        }

        public Agent? FindAgentById(string id)
        {
            return _teams.SelectMany(t => t.Agents)
                         .FirstOrDefault(a => a.Id == id);
        }
    }
}
