namespace AVALORA.Infrastructure.DbInitializer;

public interface IDbInitializer
{
    /// <summary>
    /// Initializes the database with migrations and default user/s.
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();
}

