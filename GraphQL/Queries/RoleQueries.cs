using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Query")]
public class RoleQueries
{
    public IQueryable<Role> GetRoles([Service] AppDbContext context)
    {
        return context.Roles
            .Include(r => r.RoleClaims)
                .ThenInclude(rc => rc.Claim);
    }
}