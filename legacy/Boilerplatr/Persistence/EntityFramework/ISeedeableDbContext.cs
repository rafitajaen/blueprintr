namespace Boilerplatr.Persistence.EntityFramework;

public interface ISeedeableDbContext
{
    void Seed();
    bool IsSeeded();
    void Initialize();
}
