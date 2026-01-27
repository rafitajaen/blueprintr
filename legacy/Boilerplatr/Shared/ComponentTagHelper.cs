using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplatr.Shared;

/// <summary>
/// Implements a tag helper as a Razor view as the template
/// </summary>
/// <remarks>
///     uses convention that /TagHelpers/ has razor template based views for tags
///     For a folder /TagHelpers/Foo
///     * FooTagHelper.cs -> Defines the properties with HtmlAttribute on it (derived from ViewTagHelper)
///     * default.cshtml -> Defines the template with Model=>FooTagHelper
/// </remarks>
public class ComponentBaseTagHelper : TagHelper
{
    // private string _viewPath;

    // public ComponentBaseTagHelper()
    // {
    //     var className = GetType().FullName!.Split(".").Last();
    //     _viewPath = $"/src/front/Components/{className}.cshtml";
    // }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }
    public TagHelperContent? ChildContent { get; set; }

    [HtmlAttributeName]
    public string? Id { get; set; }

    [HtmlAttributeName]
    public string? Class { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        // get child content and capture it in our model so we can insert it in our output
        ChildContent = await output.GetChildContentAsync();

        var htmlHelper = ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlHelper>();

        (htmlHelper as IViewContextAware)!.Contextualize(ViewContext);
        var content = await htmlHelper.PartialAsync(context.TagName, this);

        output.TagName = null;
        output.Content.SetHtmlContent(content);
    }
}
