using SupportChat.ChatAPI.Models;
using SupportChat.ChatAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SessionQueueService>();
builder.Services.AddHostedService<QueueMonitorService>();

// Define all teams
var teams = new List<AgentTeam>
{
    new AgentTeam
    {
        Name = "Team A",
        Agents = new()
        {
            new Agent { Id = "TL1", Name = "Lead A", Seniority = AgentSeniority.TeamLead },
            new Agent { Id = "M1", Name = "Mid A1", Seniority = AgentSeniority.Mid },
            new Agent { Id = "M2", Name = "Mid A2", Seniority = AgentSeniority.Mid },
            new Agent { Id = "J1", Name = "Junior A", Seniority = AgentSeniority.Junior }
        }
    },
    new AgentTeam
    {
        Name = "Team B",
        Agents = new()
        {
            new Agent { Id = "S1", Name = "Senior B", Seniority = AgentSeniority.Senior },
            new Agent { Id = "M3", Name = "Mid B", Seniority = AgentSeniority.Mid },
            new Agent { Id = "J2", Name = "Junior B1", Seniority = AgentSeniority.Junior },
            new Agent { Id = "J3", Name = "Junior B2", Seniority = AgentSeniority.Junior }
        }
    },
    new AgentTeam
    {
        Name = "Team C (Night Shift)",
        Agents = new()
        {
            new Agent { Id = "M4", Name = "Mid C1", Seniority = AgentSeniority.Mid },
            new Agent { Id = "M5", Name = "Mid C2", Seniority = AgentSeniority.Mid }
        }
    },
    new AgentTeam
    {
        Name = "Overflow",
        Agents = Enumerable.Range(1, 6)
            .Select(i => new Agent
            {
                Id = $"OJ{i}",
                Name = $"Overflow Junior {i}",
                Seniority = AgentSeniority.Junior
            }).ToList()
    }
};

builder.Services.AddSingleton(new AgentAssignmentService(teams));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();