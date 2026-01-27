using Boilerplatr.Emails.Abstractions;

namespace Boilerplatr.Emails;

public class BaseEmailTemplateModel : IEmailTemplateViewModel
{
    public string FromName { get; set;  }
    public string FromEmail { get; set;  }
    public string Subject { get; set;  }
    public string ProjectName { get; set; }
    public string ProjectUrl { get; set; }
    public string ProjectWebsite { get; set; }
    public string Copyright { get; set; }
    public string Linkedin { get; set; }
    public string X { get; set; }
    public string PrivacyUrl { get; set; }
    public string SupportEmail { get; set; }

    public BaseEmailTemplateModel() { }

    public BaseEmailTemplateModel(BaseEmailTemplateModel model)
    {
        FromName = model.FromName;
        FromEmail = model.FromEmail;
        Subject = model.Subject;
        ProjectName = model.ProjectName;
        ProjectUrl = model.ProjectUrl;
        ProjectWebsite = model.ProjectWebsite;
        Copyright = model.Copyright;
        Linkedin = model.Linkedin;
        X = model.X;
        PrivacyUrl = model.PrivacyUrl;
        SupportEmail = model.SupportEmail;
    }
}
