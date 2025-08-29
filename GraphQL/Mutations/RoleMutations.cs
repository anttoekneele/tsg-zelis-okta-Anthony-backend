[ExtendObjectType("Mutation")]
public class RoleMutations
{
    public async Task<bool> AssignRole(Guid userId, Guid roleId, Guid authorId, [Service] IRoleService roleService, [Service] IAuditService auditService)
    {
        await roleService.AssignRole(userId, roleId, authorId);
        return true;
    }
}