# 📞 Support Chat System (Backend)

A real-time Support Chat Backend built using **.NET 8 Web API**, designed to manage chat sessions, agent assignment, and user polling — with capacity handling, agent shifts, overflow teams, and session lifecycle management.

✅ Fully tested with **xUnit**  
✅ Real-world support chat rules and queuing  
✅ Clean architecture (Services, Models, Controllers)

---

## 🚀 Installation Guide

### 📦 Prerequisites

- Visual Studio 2022 or later
- .NET 8 SDK installed
- Git installed (optional for cloning)

### 📦 Steps

1. Clone the repository:

    ```bash
    git clone https://github.com/rdaniel10/backend-test.git
    ```

2. Open the solution file `SupportChat.sln` in Visual Studio.

3. Set `SupportChat.ChatAPI` as the startup project.

4. Build the solution (Ctrl + Shift + B).

5. Run the project (F5).

---

## 🛠️ How It Works

1. **User** sends a `POST /api/chat/create-session`.
2. **Session** is queued, pending assignment.
3. **Monitor** checks sessions every second:
   - Assigns available agents by seniority and capacity.
   - Marks sessions inactive after 3 missed polls.
4. **Agent Assignment** follows round-robin with seniority preference.
5. **Overflow Team** kicks in if the main team is full.
6. **Session Completion**:
   - Auto-completes (inactive after missed polls).
   - Or manually completed with `POST /api/chat/complete/{sessionId}`.

---

## 🎯 Features

- **Session Queue**: FIFO queue for user support sessions.
- **Polling System**: Client polls every second; session marked inactive after 3 missed polls.
- **Agent Assignment**:
  - Junior → Mid → Senior → Team Lead (Seniority Priority)
  - Round-robin assignment inside same seniority.
- **Capacity Management**:
  - Max concurrency per agent based on efficiency.
- **Session Lifecycle**:
  - Sessions auto-close when inactive.
  - Manual chat completion supported.
- **Overflow Team Handling**:
  - Extra agents assigned when main capacity is exceeded.

---

## 🛠 Tech Stack

- .NET 8 Web API
- xUnit for Unit Testing
- Moq for Dependency Mocking (Optional)
- HostedService for background monitoring
- In-Memory Queue Storage
- Minimal Console Logs for Monitoring

---

## 🧪 Unit Testing

All services are independently tested with **xUnit**.

Run unit tests using the Test Explorer in Visual Studio or via CLI:

```bash
dotnet test
```

Test coverage includes:

- SessionQueueServiceTests
- AgentAssignmentServiceTests
- QueueMonitorServiceTests

---

## 📄 API Endpoints

| Method | Endpoint | Description |
|:-------|:---------|:------------|
| POST   | `/api/chat/create-session` | Create a new chat session (userId from Form) |
| POST   | `/api/chat/poll/{sessionId}` | Client heartbeat (polling every 1s) |
| POST   | `/api/chat/complete/{sessionId}` | Manually close a session |
| GET    | `/api/chat/queue` | View all queued sessions (for admin/debug) |

---

## 💡 Recommendation for Future Enhancement

To further improve scalability and real-time capabilities, it is recommended to integrate a message broker such as **Kafka** or **RabbitMQ**.

- **Use Case**: Decouple the session creation, polling, and agent assignment processes through asynchronous event-driven architecture.
- **Benefit**: Enhance system reliability, allow distributed processing, and prepare the system for higher load and microservices adoption.

---

## 👨‍💻 Author

Rein Daniel Salosa

---

## 🎯 Final Note

This project was developed as a real-world simulation of a **Support Chat System** with full backend logic, session handling, and agent management based on real business rules.