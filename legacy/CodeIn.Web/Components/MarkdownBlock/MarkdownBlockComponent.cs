using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.MarkdownBlock;

[HtmlTargetElement(ComponentTargets.MarkdownBlock)]
public class MarkdownBlockComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public MarkdownText? Text { get; set; }

}
