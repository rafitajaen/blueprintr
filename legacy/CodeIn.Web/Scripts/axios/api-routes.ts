export class NewslettersApi
{
    private static Base = "/newsletters";
    public readonly Suscribe = NewslettersApi.Base + "/suscribe";
}

export class CandidatesApi
{
    private static Base = "/candidates";
    public readonly Submissions = CandidatesApi.Base;
}

export class QuestionsApi
{
    private static Base = "/questions";
    public readonly Submissions = QuestionsApi.Base;
}

export class LeadsApi
{
    private static Base = "/lead";
    public readonly Submissions = LeadsApi.Base + "/submissions";
}

export class AuthApi
{
    private static Base = "/auth";
    public readonly Login = AuthApi.Base + "/login";
}

export class RoutesApi
{
    public readonly Auth = new AuthApi();
    public readonly Leads = new LeadsApi();
    public readonly Questions = new QuestionsApi();
    public readonly Newsletters = new NewslettersApi();
    public readonly Candidates = new CandidatesApi();
}

export const ApiRoutes = new RoutesApi();
