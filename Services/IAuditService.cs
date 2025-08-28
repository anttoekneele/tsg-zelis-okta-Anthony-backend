public interface IAuditService
{
    Task LogLoginEvent(Guid userId, string provider);
    Task LogLogoutEvent(Guid userId);
    Task LogRoleAssignedEvent(Guid authorId, Guid affectedId, string fromRole, string toRole);
}