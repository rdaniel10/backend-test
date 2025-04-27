using SupportChat.ChatAPI.Models;
using SupportChat.ChatAPI.Services;
using Xunit;

namespace SupportChat.Tests
{
    public class SessionQueueServiceTests
    {
        [Fact]
        public void Enqueue_ShouldAddSession_ToQueue()
        {
            // Arrange
            var service = new SessionQueueService();
            var session = new ChatSession { UserId = "test-user" };

            // Act
            service.Enqueue(session);

            // Assert
            Assert.Equal(1, service.Count());
            Assert.Contains(service.GetAll(), s => s.UserId == "test-user");
        }

        [Fact]
        public void RemoveInactiveSessions_ShouldRemoveSessionsWithInactiveStatus()
        {
            // Arrange
            var service = new SessionQueueService();
            var activeSession = new ChatSession { UserId = "active-user" };
            var inactiveSession = new ChatSession { UserId = "inactive-user", Status = SessionStatus.Inactive };
            service.Enqueue(activeSession);
            service.Enqueue(inactiveSession);

            // Act
            service.RemoveInactiveSessions();

            // Assert
            Assert.Single(service.GetAll()); // Only 1 session should remain
            Assert.DoesNotContain(service.GetAll(), s => s.Status == SessionStatus.Inactive);
        }
    }
}
