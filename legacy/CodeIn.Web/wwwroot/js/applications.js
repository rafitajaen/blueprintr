import { C as Component } from "./styles-BHKG78eI.js";
import "./empty.js";
import { I as Input, B as Button, A as ActionInput, D as Dialog, a as api, b as ApiRoutes } from "./dialog-B8FD7tRf.js";
import { A as ActionArea, P as ProblemDetails } from "./problem-details-CBzP07pb.js";
class ActionFile extends Component {
  input;
  button;
  _value = "";
  get value() {
    return this._value;
  }
  get required() {
    return this.el.dataset.required?.toLowerCase() === "true";
  }
  constructor(props) {
    super({ el: props.el });
    this.clearErrors = this.clearErrors.bind(this);
    this.onInputChange = this.onInputChange.bind(this);
    this.input = new Input({
      ...props,
      el: this.el.querySelector(":scope input"),
      onInput: (e) => {
        this.clearErrors();
        if (props.onInput) props.onInput(e);
      },
      onChange: (e) => {
        this.onInputChange(e);
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
  onInputChange(e) {
    e.stopPropagation();
    const files = this.input.el.files;
    if (files && files.length > 0) {
      const file = files[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = (event) => {
          this._value = event.target.result;
        };
        reader.onerror = (error) => {
          this._value = "";
          console.error("Error reading file:", error);
        };
        reader.readAsDataURL(file);
      }
    }
  }
  setValue(value) {
    this.input.setValue(value);
  }
  clear() {
    this.input.clear();
    this.clearErrors();
  }
  clearErrors() {
    document.querySelectorAll(":scope .error").forEach((e) => e.setAttribute("hidden", "true"));
  }
  setError(type) {
    this.el.querySelector(`:scope .error[data-type="${type}"]`)?.removeAttribute("hidden");
    this.input.el.scrollIntoView({ behavior: "auto", block: "center", inline: "center" });
  }
  validate() {
    const hasErrors = this.required && !this.value;
    if (hasErrors) this.setError("Empty");
    return hasErrors;
  }
}
class JobApplications {
  // Fields
  name;
  email;
  cover;
  resume;
  // Submit button
  submit;
  // Dialog
  failed;
  get jobId() {
    return document.querySelector("article")?.dataset.jobId;
  }
  constructor() {
    this.cover = new ActionArea({
      el: document.querySelector(`[data-input-id="cover"]`)
    });
    this.email = new ActionInput({
      el: document.querySelector(`[data-input-id="email"]`)
    });
    this.name = new ActionInput({
      el: document.querySelector(`[data-input-id="full-name"]`)
    });
    this.resume = new ActionFile({
      el: document.querySelector(`[data-input-id="resume"]`)
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
      jobId: this.jobId,
      cover: this.cover.value,
      name: this.name.value,
      email: this.email.value,
      resume: this.resume.value
    };
    api.post(ApiRoutes.Candidates.Submissions, data).then(() => {
      const url = new URL(window.location.href);
      url.pathname = url.pathname.replace(/\/$/, "") + "/submitted";
      if (data.email) url.searchParams.append("email", data.email);
      window.location.href = url.toString();
    }).catch((error) => {
      console.log({ error });
      const problems = new ProblemDetails(error);
      if (problems.contains("EmailValidator", "Email")) this.email.setError("InvalidEmail");
      if (problems.contains("MaximumLengthValidator", "Cover")) this.cover.setError("TooLong");
      if (problems.contains("MaximumLengthValidator", "Name")) this.name.setError("TooLong");
      if (problems.containsTitle("QuestionSubmission.Job.Id")) this.failed.open();
      if (problems.containsTitle("CandidateSubmission.Resume.NotPdf")) this.resume.setError("Required");
    }).finally(() => this.submit.ready());
  }
  validate() {
    let hasErrors = this.cover.validate();
    hasErrors = this.email.validate() || hasErrors;
    return !hasErrors;
  }
}
new JobApplications();
