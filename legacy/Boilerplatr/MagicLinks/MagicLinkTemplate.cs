using System.Text;
using Boilerplatr.Emails;
using Boilerplatr.Emails.Renderers;
using Boilerplatr.Extensions;
using Microsoft.Extensions.Logging;
using Mjml.Net;
using RazorEngineCore;

namespace Boilerplatr.MagicLinks;

public class MagicLinkTemplateModel(string link, BaseEmailTemplateModel model) : BaseEmailTemplateModel(model)
{
    public string MagicLink { get; set; } = link;
}

public class MagicLinkTemplate
(
    MjmlRenderer mjml,
    IRazorEngine razor,
    ILogger<MagicLinkTemplate> logger
) : MjmlEmailRenderer<MagicLinkTemplateModel>(mjml, razor, logger)
{
    public override void OnCachedError(ValidationErrors errors)
    {
        logger.LogWarningUnexpectedException
        (
            serviceName: nameof(MagicLinkTemplate),
            methodName: nameof(OnCachedError),
            exceptionMessage: string.Join(';', errors.Select(e => e.ToString()))
        );
    }

    public override string RenderHtmlEmail(MagicLinkTemplateModel model)
    {
        // TODO: Externalize BasePath
        
        var path = $"src/DotnetToday.Web/Templates/{nameof(MagicLinkTemplate)}";
        var options = new MjmlOptions()
        {
            PostProcessors = [AngleSharpPostProcessor.Default]
        };

        if (TryRender(path, model, options, out var html))
        {
            return html;
        }

        return string.Empty;
    }

    public override string RenderTextEmail(MagicLinkTemplateModel model)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Welcome to {model.ProjectName}!");
        sb.AppendLine();
        sb.AppendLine("Please copy and paste this URL into your browser to sign in to your account:");
        sb.AppendLine();
        sb.AppendLine(model.MagicLink);
        sb.AppendLine();
        sb.AppendLine("You received this email because a sign-in was requested for your account.");
        sb.AppendLine("If you did not request this, you can safely ignore this message.");
        sb.AppendLine();
        sb.AppendLine($"Any questions? Please feel free to reach us at {model.SupportEmail}");
        sb.AppendLine($"© {model.Copyright}");

        return sb.ToString();
    }

}
