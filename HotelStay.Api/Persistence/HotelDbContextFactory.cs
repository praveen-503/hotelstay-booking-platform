using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelStay.Api.Persistence;

public sealed class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
{
    public HotelDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
        optionsBuilder.UseNpgsql(ConnectionStringResolver.Resolve(configuration), npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(HotelDbContext).Assembly.FullName);
        });

        return new HotelDbContext(optionsBuilder.Options);
    }
}