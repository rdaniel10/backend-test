using SupportChat.ChatAPI.Models;

namespace SupportChat.ChatAPI.Services
{
    public class SessionQueueService
    {
        private readonly Queue<ChatSession> _queue = new();

        public void Enqueue(ChatSession session) => _queue.Enqueue(session);

        public ChatSession? Dequeue() => _queue.Count > 0 ? _queue.Dequeue() : null;

        public int Count() => _queue.Count;

        public IEnumerable<ChatSession> GetAll() => _queue.ToList();

        public ChatSession? GetById(Guid id) => _queue.FirstOrDefault(s => s.Id == id);

        public void RemoveInactiveSessions()
        {
            var activeSessions = _queue.Where(s => s.Status != SessionStatus.Inactive && s.Status != SessionStatus.Closed).ToList();
            _queue.Clear();
            foreach (var session in activeSessions)
            {
                _queue.Enqueue(session);
            }
        }
    }
}
