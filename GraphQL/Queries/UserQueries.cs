using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Query")]
public class UserQueries
{
    public IQueryable<User> GetUsers([Service] AppDbContext context)
    {
        return context.Users
            .Include(u => u.Role);
    }
}