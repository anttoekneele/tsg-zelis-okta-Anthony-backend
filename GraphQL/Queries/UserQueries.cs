public class UserQueries
{
    public IQueryable<User> GetUsers([Service] AppDbContext context)
    {
        return context.Users;
    }
}