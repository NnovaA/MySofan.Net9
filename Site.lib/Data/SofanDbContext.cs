using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Site.lib;
using Site.lib.Data;
using Site.lib.Models;
using Site.lib.Sofan.Seed;
using Attribute = System.Attribute;

namespace Site.Lib.Data;

public class SofanDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public SofanDbContext(DbContextOptions<SofanDbContext> options) : base(options) { }
    public DbSet<Activity> Activity { get; set; }
    public DbSet<SofAttribute> SofAttribute { get; set; }
    public DbSet<Culture> Culture { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Localize> Localize { get; set; }
    public DbSet<LocAttributes> LocAttributes { get; set; }
    public DbSet<LocEntry> LocEntry { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<RelatedAttributes> RelatedAttributes { get; set; }
    public DbSet<RelatedEntries> RelatedEntries { get; set; }
    public DbSet<RelatedEntryAttributes> RelatedEntryAttributes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        // var cs = inDocker ? ConfigHub.DockerCs : ConfigHub.Cs;
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseMySql(Config.Cs, ServerVersion.AutoDetect(Config.Cs)).EnableSensitiveDataLogging().EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }));
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
        
        builder.Entity<LocAttributes>().HasIndex(x => x.CultureId);
        
        builder.ApplyConfiguration(new DatabaseSeeder.CultureSeeder());
        builder.ApplyConfiguration(new DatabaseSeeder.AttributeSeeder());
        builder.ApplyConfiguration(new DatabaseSeeder.LocAttributeSeeder());
        builder.ApplyConfiguration(new DatabaseSeeder.LocalizeSeeder());
        
        builder.Entity<IdentityUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        builder.Entity<Activity>(o =>
        {
            o.ToTable("Activities");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => new { e.Id, e.ActionId });
        });

        builder.Entity<Address>(o =>
        {
            o.ToTable("Address");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            
            o.HasKey(a => a.AddressId);
            o.HasOne(a => a.Entry)
                .WithMany()
                .HasForeignKey(a => a.EntryId)
                .OnDelete(DeleteBehavior.Cascade);
            o.Property(e => e.Block).HasMaxLength(256);
            o.Property(e => e.Building).HasMaxLength(256);
            o.Property(e => e.Street).HasMaxLength(256);
            o.Property(e => e.Floor).HasMaxLength(256);
            o.Property(e => e.Avenue).HasMaxLength(256);
            o.Property(e => e.OfficeNo).HasMaxLength(256);
            o.Property(e => e.ApartmentNo).HasMaxLength(256);
        });

        builder.Entity<SofAttribute>(o =>
        {
            o.ToTable("SofAttributes");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.Property(e => e.AttributeId).ValueGeneratedNever();
            
            o.HasKey(a => a.AttributeId);
            
            o.HasOne(a => a.Parent)
                .WithMany(a => a.ChildAttributes)
                .HasForeignKey(a => a.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            o.HasMany(a => a.Masters)
                .WithOne(ra => ra.Slave)
                .HasForeignKey(ra => ra.SlaveId)
                .OnDelete(DeleteBehavior.Cascade);
            
            o.HasMany(a => a.Slaves)
                .WithOne(ra => ra.Master)
                .HasForeignKey(ra => ra.MasterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RelatedAttributes>(o =>
        {
            o.ToTable("RelatedAttributes");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasOne(e => e.Master).WithMany(m => m.Slaves).HasForeignKey(a => a.MasterId).OnDelete(DeleteBehavior.Cascade);
            o.HasOne(e => e.Slave).WithMany(m => m.Masters).HasForeignKey(a=> a.SlaveId).OnDelete(DeleteBehavior.Cascade);
            o.HasKey(ra => new { ra.MasterId, ra.SlaveId });
            
            o.HasOne(ra => ra.Master)
                .WithMany(a => a.Slaves)
                .HasForeignKey(ra => ra.MasterId)
                .OnDelete(DeleteBehavior.NoAction);
            
            o.HasOne(ra => ra.Slave)
                .WithMany(a => a.Masters)
                .HasForeignKey(ra => ra.SlaveId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Permission>(o =>
        {
            o.ToTable("Permissions");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            
            o.HasKey(p => new { p.RoleId, p.EntryId });
            
            o.HasOne(p => p.Entry)
                .WithMany()
                .HasForeignKey(p => p.EntryId);
            
            o.HasOne(p => p.Role)
                .WithMany()
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Culture>(o =>
        {
            o.ToTable("Cultures");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => e.CultureId);
            o.Property(c => c.CultureId).UseCollation("utf8mb4_general_ci");
            o.Property(e => e.CultureId).HasMaxLength(2).IsFixedLength();
            o.Property(e => e.UiCulture).HasMaxLength(5).IsFixedLength();
        });
        builder.Entity<Localize>(o =>
        {
            o.ToTable("Localizes");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => new { e.ResKey, e.CultureId, e.Group });
            o.Property(e => e.CultureId).HasMaxLength(2).IsFixedLength();
            
            o.HasOne(e => e.Culture)
                .WithMany(c => c.locales)
                .HasForeignKey(e => e.CultureId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<LocAttributes>(o =>
        {
            o.ToTable("LocAttributes");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => new {e.AttributeId, e.CultureId});
            o.Property(e => e.CultureId).HasMaxLength(2).IsFixedLength();
            
            o.HasOne(e => e.Attribute)
                .WithMany(a => a.Locales)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            o.HasOne(e => e.Culture)
                .WithMany()
                .HasForeignKey(e => e.CultureId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Entry>(o =>
        {
            o.ToTable("Entries");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => e.EntryId);
        });
        builder.Entity<LocEntry>(o =>
        {
            o.ToTable("LocEntries");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => e.EntryId);
            o.Property(e => e.CultureId).HasMaxLength(2).IsFixedLength();
            o.Property(e => e.CultureId).UseCollation("utf8mb4_general_ci");
        });
        builder.Entity<RelatedEntries>(o =>
        {
            o.ToTable("RelatedEntries");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => new { e.SlaveId, e.MasterId });
            o.HasOne(e => e.MasterEntry).WithMany(p => p.Slaves).HasForeignKey(d => d.MasterId);
            o.HasOne(e => e.SlaveEntry).WithMany(p => p.Masters).HasForeignKey(d => d.SlaveId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<RelatedEntryAttributes>(o =>
        {
            o.ToTable("RelatedEntryAttributes");
            o.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");
            o.HasKey(e => new { e.AttributeId, e.EntryId });
            o.HasOne(e => e.SofAttribute).WithMany(p => p.MasterEntries).HasForeignKey(a => a.AttributeId).OnDelete(DeleteBehavior.Cascade);
            o.HasOne(e => e.Entry).WithMany(m => m.SlaveEntries).HasForeignKey(a => a.EntryId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}