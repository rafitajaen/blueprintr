using System.Diagnostics.CodeAnalysis;
using Boilerplatr.Emails.Abstractions;
using Boilerplatr.Extensions;
using Microsoft.Extensions.Logging;
using Mjml.Net;
using RazorEngineCore;

namespace Boilerplatr.Emails.Renderers;

public abstract class MjmlEmailRenderer<TViewModel>
(
    MjmlRenderer mjml,
    IRazorEngine razor,
    ILogger logger
) : IEmailRenderer<TViewModel> where TViewModel : IEmailTemplateViewModel 
{
    private readonly string[] cssAtRules =
    [
        "bottom-center", "bottom-left", "bottom-left-corner", "bottom-right", "bottom-right-corner", "charset", "counter-style",
        "document", "font-face", "font-feature-values", "import", "left-bottom", "left-middle", "left-top", "keyframes", "media",
        "namespace", "page", "right-bottom", "right-middle", "right-top", "supports", "top-center", "top-left", "top-left-corner",
        "top-right", "top-right-corner"
    ];

    private IRazorEngineCompiledTemplate<RazorEngineTemplateBase<TViewModel>>? Template = null;

    private string EscapeCssRulesInRazorTemplate(string mjmlOutput)
    {
        return cssAtRules.Aggregate
        (
            seed: mjmlOutput,
            func: (current, rule) => current.Replace($"@{rule}", $"@@{rule}")
        );
    }

    private IRazorEngineCompiledTemplate<RazorEngineTemplateBase<TViewModel>> CacheTemplate(string templateRoute, MjmlOptions? options = null)
    {
        var mjmlSource = File.ReadAllText($"{templateRoute}.mjml");
        var (mjmlOutput, errors) = mjml.Render(mjmlSource, options ?? new MjmlOptions());

        if (errors.Any())
        {
            OnCachedError(errors);
            throw new();
        }
        else
        {
            mjmlOutput = EscapeCssRulesInRazorTemplate(mjmlOutput);
            mjmlOutput = mjmlOutput.Replace("wght@0", "wght@@0");

            // Razor.AddTemplate($"{template}.cshtml", mjmlOutput);
            return razor.Compile<RazorEngineTemplateBase<TViewModel>>(mjmlOutput);
        }
    }

    protected bool TryRender(string templateRoute, TViewModel model, MjmlOptions? options, [NotNullWhen(true)] out string? html)
    {
        html = null;

        try
        {
            Template ??= CacheTemplate(templateRoute, options);
            html = Template.Run(x => x.Model = model);
        }
        catch (Exception ex)
        {
            logger.LogCriticalUnexpectedException
            (
                serviceName: nameof(MjmlEmailRenderer<TViewModel>),
                methodName: nameof(TryRender),
                exceptionMessage: ex.Message
            );
        }

        return !string.IsNullOrWhiteSpace(html);
    }

    public abstract string RenderTextEmail(TViewModel model);
    public abstract string RenderHtmlEmail(TViewModel model);

    public abstract void OnCachedError(ValidationErrors errors);

}
