using CommandLine;

namespace ProjectTimeTracker.DbUp;

public class Options
{
    [Option('c', "connectionString", Required = true, HelpText = "Connection string to the database")]
    public string ConnectionString { get; set; } = string.Empty;
}
