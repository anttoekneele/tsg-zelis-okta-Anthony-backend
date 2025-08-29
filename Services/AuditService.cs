
public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }
    public async Task LogLoginEvent(Guid userId, string provider)
    {
        var eventRecord = new SecurityEvent
        {
            Id = Guid.NewGuid(),
            EventType = "LoginSuccess",
            AuthorUserId = userId,
            AffectedUserId = userId,
            OccurredUtc = DateTime.UtcNow,
            Details = $"provider={provider}"
        };

        await _context.SecurityEvents.AddAsync(eventRecord);
        await _context.SaveChangesAsync();
    }

    public async Task LogLogoutEvent(Guid userId)
    {
        var eventRecord = new SecurityEvent
        {
            Id = Guid.NewGuid(),
            EventType = "Logout",
            AuthorUserId = userId,
            AffectedUserId = userId,
            OccurredUtc = DateTime.UtcNow,
            Details = "local sign-out"
        };

        await _context.SecurityEvents.AddAsync(eventRecord);
        await _context.SaveChangesAsync();
    }

    public async Task LogRoleAssignedEvent(Guid authorId, Guid affectedId, string fromRole, string toRole)
    {
        var eventRecord = new SecurityEvent
        {
            Id = Guid.NewGuid(),
            EventType = "RoleAssigned",
            AuthorUserId = authorId,
            AffectedUserId = affectedId,
            OccurredUtc = DateTime.UtcNow,
            Details = $"from={fromRole} to={toRole}"
        };

        await _context.SecurityEvents.AddAsync(eventRecord);
        await _context.SaveChangesAsync();
    }
}