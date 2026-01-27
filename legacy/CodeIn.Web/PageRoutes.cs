using Boilerplatr.Utils;
using CodeIn.Domain.Companies;
using CodeIn.Domain.Jobs;

namespace CodeIn.Web;

public static partial class PageRoutes
{
    private const string Root = "/";

    public const string Home = Root;
    public const string Feedback = Root + "feedback";
    public const string Privacy = Root + "privacy";
    public const string Terms = Root + "terms";
    public const string Cookies = Root + "cookies";
    public const string Contact = Root + "contact";
    public const string Error = Root + "error";
    public const string About = Root + "about";
    public const string NotFound = nameof(NotFound);


    public static class Hire
    {
        public const string Base = Root + "hire";
        public const string LeadSubmission = Base + "/submit";
        public const string LeadVerification = Base + "/verification";
    }

    public static class Blog
    {
        public const string Category = Root + "{categorySlug}";
        public const string Article = Category + "/{articleSlug}";
    }

    public static class Companies
    {
        public const string Base = Root + "companies";
        public const string Profile = Base + "/{companySlug}";
    }

    public static class Jobs
    {
        public const string Base = Root + "jobs";
        public const string ByTopic = Base + "/{topicSlug}";
        public const string Entry = Companies.Profile + "/{jobSlug}";
        public const string Question = Entry + "/question";
        public const string QuestionSubmitted = Question + "/submitted";

        public const string Application = Entry + "/application";
        public const string ApplicationSubmitted = Entry + "/submitted";
    }

    public static partial class Auth
    {
        private const string Base = Root + "auth";
        public const string Login = Base + "/login";
        public const string Verify = Base + "/verify";
        public const string MagicLink = Base + "/magic-link";
    }

    public static partial class Newsletters
    {
        public const string Base = Root + "newsletters";
        public const string Verification = Base + "/verification";
        public const string Preferences = Base + "/preferences";
        public const string Welcome = Base + "/welcome";
    }

    public static partial class Dashboard
    {
        public const string Base = "/dashboard";
        public const string Profile = Base + "/profile";
        public const string Jobs = Base + "/jobs";
    }

    public static string GetCompanyUrl(this Company company) => Companies.Profile.Replace("{companySlug}", company.Slug.Value).NormalizeUrl();
    public static string GetJobUrl(this Company company, Job job)
    {
        if (job.HasDirectApplication && !string.IsNullOrWhiteSpace(job.ApplicationUrl))
        {
            return job.ApplicationUrl;
        }

        return Jobs.Entry.Replace("{companySlug}", company.Slug.Value).Replace("{jobSlug}", job.Slug.Value).NormalizeUrl();
    }
    public static string GetJobQuestionUrl(this Company company, Job job) => Jobs.Question.Replace("{companySlug}", company.Slug.Value).Replace("{jobSlug}", job.Slug.Value).NormalizeUrl();
    public static string GetJobApplicationUrl(this Company company, Job job) => Jobs.Application.Replace("{companySlug}", company.Slug.Value).Replace("{jobSlug}", job.Slug.Value).NormalizeUrl();
}
