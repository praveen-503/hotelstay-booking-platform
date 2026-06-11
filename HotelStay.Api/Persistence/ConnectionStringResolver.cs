using Npgsql;

namespace HotelStay.Api.Persistence;

public static class ConnectionStringResolver
{
    private static readonly string[] EnvironmentVariableCandidates =
    {
        "DefaultConnection",
        "ConnectionStrings__DefaultConnection",
        "POSTGRES_CONNECTION_STRING",
        "DATABASE_URL"
    };

    public static string Resolve(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        foreach (var variable in EnvironmentVariableCandidates)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (variable == "DATABASE_URL")
                {
                    return ConvertDatabaseUrlToConnectionString(value);
                }

                return value;
            }
        }

        var host = Environment.GetEnvironmentVariable("PGHOST");
        var port = Environment.GetEnvironmentVariable("PGPORT");
        var database = Environment.GetEnvironmentVariable("PGDATABASE");
        var username = Environment.GetEnvironmentVariable("PGUSER");
        var password = Environment.GetEnvironmentVariable("PGPASSWORD");

        if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(database) && !string.IsNullOrWhiteSpace(username))
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = int.TryParse(port, out var parsedPort) ? parsedPort : 5432,
                Database = database,
                Username = username,
                Password = password,
                SslMode = SslMode.Require,
                IncludeErrorDetail = true
            };

            return builder.ConnectionString;
        }

        throw new InvalidOperationException("A PostgreSQL connection string was not found in appsettings.json or environment variables.");
    }

    private static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.Trim('/'),
            Username = Uri.UnescapeDataString(uri.UserInfo.Split(':')[0]),
            Password = uri.UserInfo.Contains(':') ? Uri.UnescapeDataString(uri.UserInfo.Split(':')[1]) : string.Empty,
            SslMode = SslMode.Require,
            IncludeErrorDetail = true
        };

        var query = uri.Query.TrimStart('?');
        if (!string.IsNullOrWhiteSpace(query))
        {
            foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var pair = part.Split('=', 2);
                if (pair.Length != 2)
                {
                    continue;
                }

                if (pair[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase) && pair[1].Equals("require", StringComparison.OrdinalIgnoreCase))
                {
                    builder.SslMode = SslMode.Require;
                }
            }
        }

        return builder.ConnectionString;
    }
}