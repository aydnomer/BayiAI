using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BayiAI.Data;

/// <summary>
/// Yalnızca "Add-Migration" ve "Update-Database" komutları için.
/// Uygulama çalışırken bağlantı dizesi appsettings.json'dan okunur.
/// </summary>
public class BayiDbContextFactory : IDesignTimeDbContextFactory<BayiDbContext>
{
    public BayiDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BayiDbContext>()
            .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=BayiAI;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new BayiDbContext(options);
    }
}
