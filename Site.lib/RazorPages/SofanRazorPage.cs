using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Site.lib.Services;
using Site.lib.ViewModels;
using Sofan.Hub;

namespace Site.lib.RazorPages
{
    public abstract class SofanRazorPage<TModel> : RazorPage<TModel>
    {
        [RazorInject] public ISofLoc OreLoc { get; set; }
        [RazorInject] public ISofWidgets OreWidgets { get; set; }
        [RazorInject] public ISofAuth OreAuth { get; set; }
        
        protected delegate HtmlString Localizer(string resourceKey, params object[] args);

        private Localizer _localizer;

        protected Localizer Localize
        {
            get
            {
                if (_localizer != null) return _localizer;
                var language = OreLoc.CultureId;
                if (language != null)
                {
                    _localizer = (resourceKey, args) =>
                    {
                        var stringResource = OreAuth.Locale(resourceKey);

                        if (stringResource == null || string.IsNullOrEmpty(stringResource))
                        {
                            return new HtmlString(resourceKey);
                        }

                        return new HtmlString((args == null || args.Length == 0)
                            ? stringResource
                            : string.Format(stringResource, args));
                    };
                }
                return _localizer;
            }
        }

        protected List<VmEntry> AllPages => OreLoc.ListPages();
        protected string UrlSite => ConfigHub.Site;
        protected string Ticks => DateTime.Now.Ticks.ToString();
        protected string CultureId => OreLoc.CultureId;
        protected string DateFormat => "dd/MM/yyyy";
    }
}