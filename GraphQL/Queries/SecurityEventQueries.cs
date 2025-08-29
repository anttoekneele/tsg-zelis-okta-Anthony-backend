using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Query")]
public class SecurityEventQueries
{
    public IQueryable<SecurityEvent> GetSecurityEvents([Service] AppDbContext context)
    {
        return context.SecurityEvents
            .Include(e => e.AuthorUser)
            .Include(e => e.AffectedUser)
            .OrderByDescending(e => e.OccurredUtc);
    }
}