namespace Calori.Persistence
{
    public class DbInitializer
    {
        public void Initialize(CaloriDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
