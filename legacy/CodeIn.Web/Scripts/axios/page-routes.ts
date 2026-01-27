export class AuthPages
{
    private static Base = "/auth";
    public readonly Login = AuthPages.Base + "/login";
    public readonly Verify = AuthPages.Base + "/verify";
}

export class RoutesPages
{
    public readonly Auth = new AuthPages();
}

export const PageRoutes = new RoutesPages();
