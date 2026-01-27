namespace Boilerplatr.Emails.Abstractions;

public interface IEmailRenderer<TViewModel>
where TViewModel : IEmailTemplateViewModel 
{
    string RenderTextEmail(TViewModel model);
    string RenderHtmlEmail(TViewModel model);
}
