import { B as Button } from "./dialog-B8FD7tRf.js";
import { C as Component } from "./styles-BHKG78eI.js";
class TextArea extends Component {
  default;
  constructor(props) {
    super(props);
    this.default = props.default || "";
    if (props.onFocus) this.el.addEventListener("focus", props.onFocus);
    if (props.onInput) this.el.addEventListener("input", props.onInput);
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
class ActionArea extends Component {
  textarea;
  button;
  get value() {
    return this.textarea.el.value;
  }
  get required() {
    return this.el.dataset.required?.toLowerCase() === "true";
  }
  constructor(props) {
    super({ el: props.el });
    this.clearErrors = this.clearErrors.bind(this);
    this.textarea = new TextArea({
      ...props,
      el: this.el.querySelector(":scope textarea"),
      onInput: (e) => {
        this.clearErrors();
        if (props.onInput) props.onInput(e);
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
    this.textarea.setValue(value);
  }
  clear() {
    this.textarea.clear();
    this.clearErrors();
  }
  clearErrors() {
    document.querySelectorAll(":scope .error").forEach((e) => e.setAttribute("hidden", "true"));
  }
  setError(type) {
    this.el.querySelector(`:scope .error[data-type="${type}"]`)?.removeAttribute("hidden");
    this.textarea.el.scrollIntoView({ behavior: "auto", block: "center", inline: "center" });
  }
  validate() {
    const hasErrors = this.required && !this.value;
    if (hasErrors) this.setError("Empty");
    return hasErrors;
  }
}
class Problem {
  code = "";
  description = "";
  field = "";
  title = "";
  type = "";
  constructor(p) {
    this.code = Problem.extract(p.code);
    this.description = Problem.extract(p.description);
    this.field = Problem.extract(p.field);
    this.title = Problem.extract(p.title);
    this.type = Problem.extract(p.type);
  }
  static extract(x) {
    return typeof x === "string" ? x : "";
  }
}
class ProblemDetails {
  problems = [];
  constructor(e) {
    const data = e?.response?.data;
    if (!data) return;
    else if (data.errors && Array.isArray(data.errors)) {
      this.problems = data.errors.map((x) => new Problem(x));
    } else {
      this.problems = [new Problem(data)];
    }
  }
  contains(code, field) {
    return code.length > 0 && field.length > 0 && this.problems.some((p) => p.code === code && p.field === field);
  }
  containsTitle(title) {
    return title.length > 0 && this.problems.some((p) => p.title === title);
  }
}
export {
  ActionArea as A,
  ProblemDetails as P
};
