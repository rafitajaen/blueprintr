using Boilerplatr.Shared;
using CodeIn.Web.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.CardContent;

[HtmlTargetElement(ComponentTargets.Card.Content)]
public class CardContentComponent : ComponentBaseTagHelper;
