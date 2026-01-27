import "./styles-BHKG78eI.js";
import { A as ActionInput, D as Dialog, a as api, b as ApiRoutes } from "./dialog-B8FD7tRf.js";
import { P as PageRoutes } from "./page-routes-CpATD2ep.js";
class Login {
  email;
  failed;
  constructor() {
    this.email = new ActionInput({
      el: document.querySelector(".email-action"),
      button: document.getElementById("submit"),
      onAction: this.onSubmit.bind(this)
    });
    this.failed = new Dialog({
      el: document.getElementById("login-failed")
    });
  }
  onSubmit(e) {
    e?.preventDefault();
    this.email.button?.load();
    const parsed = new URL(window.location.href);
    const email = this.email.value;
    api.post(ApiRoutes.Auth.Login, {
      email,
      returnUrl: parsed.searchParams.get("returnUrl") || void 0,
      username: parsed.searchParams.get("username") || void 0
    }).then(() => {
      window.location.href = `${PageRoutes.Auth.Verify}?email=${email}`;
    }).catch((error) => {
      if (error.response?.status === 400) {
        this.email.setError("Email.Invalid");
        return;
      }
      this.failed.open();
    }).finally(() => this.email.button?.ready());
  }
}
new Login();
