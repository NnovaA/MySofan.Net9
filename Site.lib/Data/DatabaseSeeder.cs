using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Site.lib.Models;
using Site.lib.Sofan.Seed;
using Site.lib.ViewModels;

namespace Site.lib.Data
{
    public static class DatabaseSeeder
    {
        public class CultureSeeder : IEntityTypeConfiguration<Culture>
        {
            public void Configure(EntityTypeBuilder<Culture> e)
            {
                e.HasData(
                    new Culture { CultureId = "en", Name = "English", UiCulture = "en-US", IsPrimary = true, IsAdmin = true, IsPublic = true, Order = 1, Flag = "flag-icon-us.svg" },
                    new Culture { CultureId = "ar", Name = "Arabic", UiCulture = "ar-KW", IsPrimary = false, IsAdmin = false, IsPublic = true, Order = 2, Flag = "flag-icon-sy.svg" }
                );
            }
        }

        public class AttributeSeeder : IEntityTypeConfiguration<SofAttribute>
        {
            private static void AddAttribute<T, TP>(List<SofAttribute> seedData)
                where TP : class
            {
                byte index = 1;
                foreach (var property in typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    if (property.GetValue(null) is not TP prop) continue;
                    seedData.Add(new SofAttribute
                    {
                        AttributeId = ((dynamic)prop).PropId,
                        ParentId = ((dynamic)prop).ParentId,
                        Order = index++,
                        Status = 1
                    });
                }
            }

            public void Configure(EntityTypeBuilder<SofAttribute> e)
            {
                var seedData = new List<SofAttribute>();
                
                AddAttribute<SeedAttribute, VmInitialProperty>(seedData);
                AddAttribute<SeedStatic, VmInitialProperty>(seedData);
                AddAttribute<ConfigCountries, VmInitialProperty>(seedData);
                AddAttribute<SeedSecurity, VmInitialProperty>(seedData);
                AddAttribute<SeedEntryTypes, VmInitialProperty>(seedData);
                AddAttribute<SeedTypes, VmInitialProperty>(seedData);
                AddAttribute<SeedContact, VmInitialProperty>(seedData);
                AddAttribute<SeedSocialLinks, VmInitialProperty>(seedData);
                AddAttribute<SeedEntryContentTypes, VmInitialProperty>(seedData);

                // Remove any potential duplicates
                var uniqueData = seedData
                    .GroupBy(x => x.AttributeId)
                    .Select(g => g.First())
                    .ToList();

                e.HasData(uniqueData);
            }
        }

        public class LocAttributeSeeder : IEntityTypeConfiguration<LocAttributes>
        {
            public void Configure(EntityTypeBuilder<LocAttributes> builder)
            {
                // First ensure the composite key is properly configured
                builder.HasKey(x => new { x.AttributeId, x.CultureId });
            
                // Then seed data ensuring no duplicates
                var seedData = new List<LocAttributes>();
            
                AddSeedData<SeedAttribute, VmInitialProperty>(seedData);
                AddSeedData<SeedStatic, VmInitialProperty>(seedData);
                // Add other seed classes...
            
                // Remove any potential duplicates
                var uniqueData = seedData
                    .GroupBy(x => new { x.AttributeId, x.CultureId })
                    .Select(g => g.First())
                    .ToList();
            
                builder.HasData(uniqueData);
            }
        
            private void AddSeedData<T, TP>(List<LocAttributes> seedData) where TP : class
            {
                foreach (var property in typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    if (property.GetValue(null) is not TP prop) continue;
                
                    dynamic dynamicProp = prop;
                    long attributeId = dynamicProp.PropId;
                
                    // English version
                    seedData.Add(new LocAttributes
                    {
                        AttributeId = attributeId,
                        CultureId = "en",
                        Name = dynamicProp.PropEnName,
                        Misc001 = dynamicProp.Misc001,
                        Misc002 = dynamicProp.Misc002,
                        Misc003 = dynamicProp.Misc003
                    });
                
                    // Arabic version
                    seedData.Add(new LocAttributes
                    {
                        AttributeId = attributeId,
                        CultureId = "ar",
                        Name = dynamicProp.PropArName,
                        Misc001 = dynamicProp.Misc001,
                        Misc002 = dynamicProp.Misc002,
                        Misc003 = dynamicProp.Misc003
                    });
                }
            }
        }

        public class LocalizeSeeder : IEntityTypeConfiguration<Localize>
        {
            public void Configure(EntityTypeBuilder<Localize> builder)
            {
                // First ensure the composite key is properly configured
                builder.HasKey(x => new { x.ResKey, x.CultureId, x.Group });
            
                // Then seed data ensuring no duplicates
                var seedData = new List<Localize>();
            
                AddSeedData<ConfigCountries, VmInitialProperty>(seedData);
            
                // Remove any potential duplicates
                var uniqueData = seedData
                    .GroupBy(x => new { x.ResKey, x.CultureId, x.Group })
                    .Select(g => g.First())
                    .ToList();
            
                builder.HasData(uniqueData);
            }
        
            private void AddSeedData<T, TP>(List<Localize> seedData) where TP : class
            {
                foreach (var property in typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    if (property.GetValue(null) is not TP prop) continue;
                
                    dynamic dynamicProp = prop;
                    string resKey = dynamicProp.PropId.ToString();
                
                    // English version
                    seedData.Add(new Localize
                    {
                        ResKey = resKey,
                        CultureId = "en",
                        Group = "Countries",
                        Value = dynamicProp.PropEnName
                    });
                
                    // Arabic version
                    seedData.Add(new Localize
                    {
                        ResKey = resKey,
                        CultureId = "ar",
                        Group = "Countries",
                        Value = dynamicProp.PropArName
                    });
                }
            }
        }
    }
}