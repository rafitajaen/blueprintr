import "./styles-BHKG78eI.js";
import "./empty.js";
import { A as ActionInput, B as Button, D as Dialog, a as api, b as ApiRoutes } from "./dialog-B8FD7tRf.js";
import { A as ActionArea, P as ProblemDetails } from "./problem-details-CBzP07pb.js";
class JobQuestion {
  // Fields
  email;
  description;
  // Submit button
  submit;
  // Dialog
  failed;
  get jobId() {
    return document.querySelector("article")?.dataset.jobId;
  }
  constructor() {
    this.description = new ActionArea({
      el: document.querySelector(`[data-input-id="description"]`)
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
      jobId: this.jobId,
      question: this.description.value,
      email: this.email.value
    };
    api.post(ApiRoutes.Questions.Submissions, data).then(() => {
      const url = new URL(window.location.href);
      url.pathname = url.pathname.replace(/\/$/, "") + "/submitted";
      if (data.email) url.searchParams.append("email", data.email);
      window.location.href = url.toString();
    }).catch((error) => {
      console.log({ error });
      const problems = new ProblemDetails(error);
      if (problems.contains("EmailValidator", "Email")) this.email.setError("InvalidEmail");
      if (problems.contains("MaximumLengthValidator", "Question")) this.description.setError("TooLong");
      if (problems.containsTitle("QuestionSubmission.Job.Id")) this.failed.open();
    }).finally(() => this.submit.ready());
  }
  validate() {
    let hasErrors = this.description.validate();
    hasErrors = this.email.validate() || hasErrors;
    return !hasErrors;
  }
}
new JobQuestion();
