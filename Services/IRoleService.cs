public interface IRoleService
{
    Task AssignRole(Guid userId, Guid roleId, Guid authorId);
}