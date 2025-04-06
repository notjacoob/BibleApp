namespace BibleApp.data;

public class DbAccessor(IHostEnvironment hostEnvironment, IConfiguration configuration)
{
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