using Boilerplatr.Shared;
using CodeIn.Web.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.Modal;

[HtmlTargetElement(ComponentTargets.Modal.Base)]
public class ModalComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? Title { get; set; }

    [HtmlAttributeName]
    public string? Subtitle { get; set; }

    [HtmlAttributeName]
    public bool? ShowClose { get; set; }

    public bool HasTitle() => !string.IsNullOrWhiteSpace(Title);
    public bool HasSubtitle() => !string.IsNullOrWhiteSpace(Subtitle);
    public bool HasHeader() => HasTitle() || HasSubtitle();
}
