public class SecurityEventQueries
{
    public IQueryable<SecurityEvent> GetSecurityEvents([Service] AppDbContext context)
    {
        return context.SecurityEvents;
    }
}