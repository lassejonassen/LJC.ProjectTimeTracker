using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProjectTimeTracker.Infrastructure.Persistence.DbContexts;

namespace ProjectTimeTracker.DbUp;

internal sealed class DbContextFactory(string connectionString) : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var dbContextbuilder = new DbContextOptionsBuilder<ApplicationDbContext>(); 
        dbContextbuilder.UseSqlServer(connectionString, x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        return new ApplicationDbContext(dbContextbuilder.Options);
    }
}
