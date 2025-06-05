using Microsoft.EntityFrameworkCore;
using Site.Lib.Data;
using Site.lib.DTO;
using Site.lib.Models;

namespace Site.lib.Services;

public interface IMyService
{
    List<SofAttribute> ChildAttributes(long parentId);
    SofAttribute AddAttribute(SofAttribute attribute);
    SofAttribute GetAttribute(long attributeId);
    SofAttribute UpdateAttribute(SofAttribute attribute);
    List<SofAttribute> SearchAttribute(string name);
    Task<AttributeDTO?> GetAttributeDtoAsync(long attributeId);
    Task<List<AttributeDTO>> ChildAttributesDtoAsync(long parentId);
    bool DeleteAttribute(long attributeId);
    
}

public class MyService(SofanDbContext db) : IMyService
{
    public List<SofAttribute> ChildAttributes(long parentId)
    {
        var query = db.SofAttribute
            .Where(x => x.ParentId == parentId)
            .Include(x => x.Locales)
            .Include(x => x.Parent)
            .Include(x => x.Parent.Locales).Select(c => new SofAttribute
            {
                AttributeId = c.AttributeId,
                Name = c.GetName,
                Misc001 = c.GetMis001,
                Misc002 = c.GetMis001,
                Misc003 = c.GetMis001
            }).ToList();
        return query;
    }

    public SofAttribute AddAttribute(SofAttribute attribute)
    {
        db.SofAttribute.Add(attribute);
        db.SaveChanges();
        return attribute;
    }

    public bool DeleteAttribute(long attributeId)
    {
        var attribute = db.SofAttribute.Find(attributeId);
        if (attribute == null)
        {
            return false;
        }
        db.SofAttribute.Remove(attribute);
        db.SaveChanges();
        return true;
    }

    public SofAttribute GetAttribute(long attributeId)
    {
        return db.SofAttribute
            .Include(x => x.Locales)
            .Include(x => x.Parent)
            .FirstOrDefault(x => x.AttributeId == attributeId);
    }

    public SofAttribute UpdateAttribute(SofAttribute attribute)
    {
        db.SofAttribute.Update(attribute);
        db.SaveChanges();
        return attribute;
    }

    public List<SofAttribute> SearchAttribute(string name)
    {
        return db.SofAttribute
            .Where(x => x.Name.Contains(name))
            .Include(x => x.Locales)
            .ToList();
    }
    
    public async Task<AttributeDTO?> GetAttributeDtoAsync(long attributeId)
    {
        return await db.SofAttribute
            .Where(x => x.AttributeId == attributeId)
            .Select(x => new AttributeDTO
            {
                Id = x.AttributeId,
                Name = x.GetName,  // Map from SofAttribute's computed property
                Misc001 = x.GetMis001,
                Misc002 = x.GetMis002,  // Fix if these are meant to be different
                Misc003 = x.GetMis003
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<AttributeDTO>> ChildAttributesDtoAsync(long parentId)
    {
        return await db.SofAttribute
            .Where(x => x.ParentId == parentId)
            .Select(x => new AttributeDTO
            {
                Id = x.AttributeId,
                Name = x.GetName,
                Misc001 = x.GetMis001,
                Misc002 = x.GetMis002,
                Misc003 = x.GetMis003
            })
            .AsNoTracking()
            .ToListAsync();
    }
}