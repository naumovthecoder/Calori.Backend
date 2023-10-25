namespace Calori.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(CaloriDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
