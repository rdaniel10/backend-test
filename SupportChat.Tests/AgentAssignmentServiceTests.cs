using SupportChat.ChatAPI.Models;
using SupportChat.ChatAPI.Services;
using Xunit;
using System.Collections.Generic;

namespace SupportChat.Tests
{
    public class AgentAssignmentServiceTests
    {
        private AgentAssignmentService CreateService()
        {
            var teams = new List<AgentTeam>
            {
                new AgentTeam
                {
                    Name = "Team A",
                    Agents = new List<Agent>
                    {
                        new Agent { Id = "J1", Name = "Junior 1", Seniority = AgentSeniority.Junior },
                        new Agent { Id = "M1", Name = "Mid 1", Seniority = AgentSeniority.Mid },
                        new Agent { Id = "S1", Name = "Senior 1", Seniority = AgentSeniority.Senior }
                    }
                }
            };

            return new AgentAssignmentService(teams);
        }

        [Fact]
        public void AssignAgent_ShouldAssignToJuniorFirst()
        {
            // Arrange
            var service = CreateService();
            var session = new ChatSession { UserId = "test-user" };

            // Act
            var agent = service.AssignAgent(session);

            // Assert
            Assert.NotNull(agent);
            Assert.Equal(AgentSeniority.Junior, agent!.Seniority);
            Assert.Contains(session.Id.ToString(), agent.AssignedSessionIds);
        }

        [Fact]
        public void AssignAgent_ShouldAssignToMid_WhenJuniorsAreFull()
        {
            // Arrange
            var service = CreateService();
            var junior = service.FindAgentById("J1");

            // Simulate junior agent reaching max concurrency
            for (int i = 0; i < junior!.MaxConcurrency; i++)
            {
                junior.AssignedSessionIds.Add(System.Guid.NewGuid().ToString());
            }

            var session = new ChatSession { UserId = "user2" };

            // Act
            var assignedAgent = service.AssignAgent(session);

            // Assert
            Assert.NotNull(assignedAgent);
            Assert.Equal(AgentSeniority.Mid, assignedAgent.Seniority);
            Assert.Contains(session.Id.ToString(), assignedAgent.AssignedSessionIds);
        }

        [Fact]
        public void AssignAgent_ShouldReturnNull_WhenAllAgentsAtCapacity()
        {
            // Arrange
            var service = CreateService();

            // Fill all agents to capacity
            foreach (var agent in service.GetAllAgents())
            {
                for (int i = 0; i < agent.MaxConcurrency; i++)
                {
                    agent.AssignedSessionIds.Add(System.Guid.NewGuid().ToString());
                }
            }

            var session = new ChatSession { UserId = "user3" };

            // Act
            var assignedAgent = service.AssignAgent(session);

            // Assert
            Assert.Null(assignedAgent);
        }
    }
}
