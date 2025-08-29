public class RoleQueries
{
    public IQueryable<Role> GetRoles([Service] AppDbContext context)
    {
        return context.Roles;
    }
}