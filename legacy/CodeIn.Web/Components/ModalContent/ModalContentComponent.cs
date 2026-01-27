using Boilerplatr.Shared;
using CodeIn.Web.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.ModalContent;

[HtmlTargetElement(ComponentTargets.Modal.Content)]
public class ModalContentComponent : ComponentBaseTagHelper;
