
using Microsoft.EntityFrameworkCore;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public RoleService(AppDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }
    public async Task AssignRole(Guid userId, Guid roleId, Guid authorId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null) throw new Exception("User not found");

        var oldRole = user.Role;
        var newRole = await _context.Roles.FindAsync(roleId);
        if (newRole == null) throw new Exception("Role not found");

        user.RoleId = roleId;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        await _auditService.LogRoleAssignedEvent(authorId, userId, oldRole.Name, newRole.Name);
    }
}