using Sofan.Hub;

namespace Site.lib.RazorPages
{
    public abstract class HubRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        protected string UrlSite => ConfigHub.Site;
        protected string Ticks => DateTime.Now.Ticks.ToString();
    }
}