using Sofan.Hub;

namespace Site.lib.Sofan.Seed;

public static class GuidHelper
{
    public static Guid EntryIdPadRight(long baseId, int id)
    {
        return Guid.Parse(
            $"{ConfigHub.GuidPattern}" +
            $"-{long.Parse(baseId.ToString().PadRight(12, '0')) + id}");
    }
}