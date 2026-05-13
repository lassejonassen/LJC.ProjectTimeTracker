using CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectTimeTracker.DbUp;

var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Parsing CLI arguments."),
var options = Parser.Default.ParseArguments<Options>(args);

string connectionString = options.Value.ConnectionString;

var contextFactory = new DbContextFactory(connectionString);
using var context = contextFactory.CreateDbContext(args);

logger.LogInformation("Checking for pending migrations.");
if (context.Database.GetPendingMigrations().Any())
{
    logger.LogInformation("Found pending migrations.");
    try
    {
        logger.LogInformation("Applying pending migrations.");
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        logger.LogError(e, "Applying pending migrations failed.");
        throw;
    }
}
else
{
    logger.LogInformation("No pending migrations found");
}