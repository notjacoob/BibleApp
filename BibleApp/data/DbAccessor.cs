namespace BibleApp.data;
/// <summary>
/// dedicated class to provide DB strings for different environments
/// </summary>
/// <param name="hostEnvironment">injected</param>
/// <param name="configuration">injected</param>
public class DbAccessor(IHostEnvironment hostEnvironment, IConfiguration configuration)
{
    /// <summary>
    /// gets a connection string from appsettings.json based on the environment
    /// </summary>
    /// <returns>either the development or production connection strings</returns>
    public string? GetConnectionString()
    {
        string connStr;
        if (Environment.GetEnvironmentVariable("APP_IN_PROD") != null)
        {
            connStr = "prod";
        }
        else
        {
            connStr = "local";
        }

        return configuration.GetConnectionString($"{connStr}");
    }
}