using Microsoft.AspNetCore.Mvc.Razor;

namespace CodeIn.Web;

public class LocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // You can inject dynamic values if you want (per tenant, theme, culture, etc.)
    }

    public IEnumerable<string> ExpandViewLocations
    (
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations
    )
    {
        // {2} = Area {1} = Controller, {0} = View
        var customLocations = new[]
        {
            "/Layouts/{0}.cshtml",   
            "/Views/{1}/{0}.cshtml",
            "/Sections/{0}/{0}.cshtml",
            "/Dialogs/{0}.cshtml",
            "/Components/{0}/{0}.cshtml",
            "/Icons/{0}.cshtml",
            "/Logos/{0}.cshtml",
            "/Shared/{0}.cshtml",
        };

        // Combine the custom routes with the default ones
        return customLocations.Concat(viewLocations);
    }
}
