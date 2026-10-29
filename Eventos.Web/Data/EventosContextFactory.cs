using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Eventos.Web.Data;

// Las herramientas de migración construyen el contexto sin ejecutar la carga inicial.
public class EventosContextFactory : IDesignTimeDbContextFactory<EventosContext>
{
    public EventosContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        var options = new DbContextOptionsBuilder<EventosContext>()
            .UseSqlServer(config.GetConnectionString("EventosContext")).Options;
        return new EventosContext(options);
    }
}
