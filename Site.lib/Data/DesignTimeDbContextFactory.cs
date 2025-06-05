using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Site.lib;

namespace Site.Lib.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SofanDbContext>
{
    public SofanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SofanDbContext>();
        optionsBuilder.UseMySql(Config.Cs, ServerVersion.AutoDetect(Config.Cs));

        return new SofanDbContext(optionsBuilder.Options);
    }
} 