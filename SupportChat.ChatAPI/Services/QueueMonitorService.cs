using SupportChat.ChatAPI.Models;

namespace SupportChat.ChatAPI.Services
{
    public class QueueMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SessionQueueService _queueService;
        private readonly AgentAssignmentService _assignmentService;

        public QueueMonitorService(IServiceScopeFactory scopeFactory, SessionQueueService queueService, AgentAssignmentService assignmentService)
        {
            _scopeFactory = scopeFactory;
            _queueService = queueService;
            _assignmentService = assignmentService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var sessions = _queueService.GetAll();

                foreach (var session in sessions)
                {
                    var timeSinceLastPoll = DateTime.UtcNow - session.LastPolledAt;

                    if (timeSinceLastPoll.TotalSeconds > 1)
                    {
                        session.MissedPolls++;

                        Console.WriteLine($"[Monitor] Session {session.Id} | Missed: {session.MissedPolls} | Status: {session.Status}");

                        if (session.MissedPolls >= 3)
                        {
                            session.Status = SessionStatus.Inactive;
                            Console.WriteLine($"[Monitor] Session {session.Id} marked INACTIVE due to missed polls.");
                        }
                    }

                    if (session.Status == SessionStatus.Pending && session.AssignedAgentId == null)
                    {
                        var assigned = _assignmentService.AssignChat(session);
                        if (assigned)
                        {
                            session.Status = SessionStatus.Assigned;
                            Console.WriteLine($"[Assignment] Session {session.Id} assigned to Agent {session.AssignedAgentId}");
                        }
                    }

                    if (session.Status == SessionStatus.Active && session.AssignedAgentId == null)
                    {
                        var agent = _assignmentService.AssignAgent(session);
                        if (agent != null)
                        {
                            session.Status = SessionStatus.Assigned;
                            session.AssignedAgentId = agent.Id;
                            Console.WriteLine($"[Assignment] Session {session.Id} assigned to Agent {agent.Name}");
                        }
                    }

                }

                _queueService.RemoveInactiveSessions();

                await Task.Delay(1000, stoppingToken); // This is to monitor every 1s
            }
        }
    }
}
