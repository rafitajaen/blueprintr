import { C as Component } from "./styles-BHKG78eI.js";
import "./empty.js";
import { B as Button, A as ActionInput, D as Dialog, a as api, b as ApiRoutes } from "./dialog-B8FD7tRf.js";
import { A as ActionArea, P as ProblemDetails } from "./problem-details-CBzP07pb.js";
import { P as PageRoutes } from "./page-routes-CpATD2ep.js";
class Select extends Component {
  default;
  constructor(props) {
    super(props);
    this.default = props.default || "";
    if (props.onFocus) this.el.addEventListener("focus", props.onFocus);
    if (props.onChange) this.el.addEventListener("change", props.onChange);
    if (props.onBlur) this.el.addEventListener("blur", props.onBlur);
    if (props.onKeyDown) this.el.addEventListener("keydown", props.onKeyDown);
  }
  clear() {
    this.el.value = this.default;
  }
  setValue(value) {
    this.el.value = value === void 0 ? this.default : value;
  }
}
class ActionSelect extends Component {
  select;
  button;
  get value() {
    return this.select.el.value;
  }
  constructor(props) {
    super({ el: props.el });
    this.clearErrors = this.clearErrors.bind(this);
    this.select = new Select({
      ...props,
      el: this.el.querySelector(":scope select"),
      onChange: (e) => {
        this.clearErrors();
        if (props.onChange) props.onChange(e);
      }
    });
    const button = props.button || this.el.querySelector(":scope button");
    if (button) {
      this.button = new Button({
        el: button,
        onClick: props.onAction
      });
    }
  }
  setValue(value) {
    this.select.setValue(value);
  }
  clear() {
    this.select.clear();
    this.clearErrors();
  }
  clearErrors() {
    document.querySelectorAll(":scope .error").forEach((e) => e.setAttribute("hidden", "true"));
  }
  setError(type) {
    this.el.querySelector(`:scope .error[data-type="${type}"]`)?.removeAttribute("hidden");
  }
}
class Hire {
  // Company
  company;
  hq;
  website;
  // Job
  role;
  location;
  arrangement;
  salary;
  description;
  // Contact person
  name;
  email;
  // Submit button
  submit;
  // Dialog
  failed;
  constructor() {
    this.company = new ActionInput({
      el: document.querySelector(`[data-input-id="company"]`)
    });
    this.hq = new ActionInput({
      el: document.querySelector(`[data-input-id="hq"]`)
    });
    this.website = new ActionInput({
      el: document.querySelector(`[data-input-id="website"]`)
    });
    this.role = new ActionInput({
      el: document.querySelector(`[data-input-id="role"]`)
    });
    this.location = new ActionInput({
      el: document.querySelector(`[data-input-id="location"]`)
    });
    this.arrangement = new ActionSelect({
      el: document.querySelector(`[data-input-id="arrangement"]`)
    });
    this.salary = new ActionSelect({
      el: document.querySelector(`[data-input-id="salary"]`)
    });
    this.description = new ActionArea({
      el: document.querySelector(`[data-input-id="description"]`)
    });
    this.name = new ActionInput({
      el: document.querySelector(`[data-input-id="name"]`)
    });
    this.email = new ActionInput({
      el: document.querySelector(`[data-input-id="email"]`)
    });
    this.submit = new Button({
      el: document.getElementById("submit"),
      onClick: this.onSubmit.bind(this)
    });
    this.failed = new Dialog({
      el: document.getElementById("submit-failed")
    });
  }
  onSubmit(e) {
    e?.preventDefault();
    if (!this.validate()) return;
    this.submit.load();
    const data = {
      company: this.company.value,
      hq: this.hq.value,
      website: this.website.value,
      role: this.role.value,
      location: this.location.value,
      arrangement: this.arrangement.value,
      salary: this.salary.value,
      description: this.description.value,
      name: this.name.value,
      email: this.email.value
    };
    api.post(ApiRoutes.Leads.Submissions, data).then(() => {
      window.location.href = `${PageRoutes.Auth.Verify}?email=${data.email}`;
    }).catch((error) => {
      console.log({ error });
      const problems = new ProblemDetails(error);
      if (problems.contains("EmailValidator", "Email")) this.email.setError("InvalidEmail");
      if (problems.containsTitle("CompanySubmission.Email.AlreadyInUse")) this.email.setError("AlreadyInUse");
      if (problems.containsTitle("CompanySubmission.UnavailableService")) this.failed.open();
    }).finally(() => this.submit.ready());
  }
  validate() {
    let hasErrors = this.company.validate();
    hasErrors = this.hq.validate() || hasErrors;
    hasErrors = this.website.validate() || hasErrors;
    hasErrors = this.role.validate() || hasErrors;
    hasErrors = this.location.validate() || hasErrors;
    hasErrors = this.description.validate() || hasErrors;
    hasErrors = this.name.validate() || hasErrors;
    hasErrors = this.email.validate() || hasErrors;
    return !hasErrors;
  }
}
new Hire();
