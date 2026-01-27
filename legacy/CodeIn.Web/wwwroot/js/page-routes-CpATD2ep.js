class AuthPages {
  static Base = "/auth";
  Login = AuthPages.Base + "/login";
  Verify = AuthPages.Base + "/verify";
}
class RoutesPages {
  Auth = new AuthPages();
}
const PageRoutes = new RoutesPages();
export {
  PageRoutes as P
};
