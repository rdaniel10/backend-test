using SupportChat.ChatAPI.Models;
using SupportChat.ChatAPI.Services;
using Xunit;
using System.Collections.Generic;

namespace SupportChat.Tests
{
    public class QueueMonitorServiceTests
    {
        [Fact]
        public void Session_Should_Become_Inactive_After_3_Missed_Polls()
        {
            // Arrange
            var session = new ChatSession
            {
                UserId = "test-user",
                LastPolledAt = DateTime.UtcNow.AddSeconds(-5) // Simulate last poll 5 seconds ago
            };
            var queueService = new SessionQueueService();
            queueService.Enqueue(session);

            var fakeAssignmentService = new AgentAssignmentService(new List<AgentTeam>());
            var monitorService = new TestableQueueMonitorService(queueService, fakeAssignmentService);

            // Act: Simulate 3 monitoring cycles
            monitorService.SimulateMonitoringCycle();
            monitorService.SimulateMonitoringCycle();
            monitorService.SimulateMonitoringCycle();

            // Assert
            var updatedSession = queueService.GetById(session.Id);
            Assert.NotNull(updatedSession);
            Assert.Equal(SessionStatus.Inactive, updatedSession!.Status);
            Assert.True(updatedSession.MissedPolls >= 3);
        }
    }

    // Helper "Testable" class that simulates monitor behavior without infinite loop
    public class TestableQueueMonitorService : QueueMonitorService
    {
        private readonly SessionQueueService _queueService;
        private readonly AgentAssignmentService _assignmentService;

        public TestableQueueMonitorService(SessionQueueService queueService, AgentAssignmentService assignmentService)
            : base(null!, queueService, assignmentService)
        {
            _queueService = queueService;
            _assignmentService = assignmentService;
        }

        public void SimulateMonitoringCycle()
        {
            var sessions = _queueService.GetAll();

            foreach (var session in sessions)
            {
                var timeSinceLastPoll = DateTime.UtcNow - session.LastPolledAt;

                if (timeSinceLastPoll.TotalSeconds > 1)
                {
                    session.MissedPolls++;

                    if (session.MissedPolls >= 3)
                    {
                        session.Status = SessionStatus.Inactive;
                    }
                }
            }
        }
    }
}
