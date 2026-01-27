import "./styles-BHKG78eI.js";
import { A as ActionInput, D as Dialog, a as api, b as ApiRoutes } from "./dialog-B8FD7tRf.js";
window.showMore = (id) => {
  [...document.querySelectorAll(`[data-hide="${id}"]`)].forEach((x) => x.classList.remove("hide"));
  document.getElementById(id)?.remove();
};
window.toggleAccordion = (id) => {
  document.querySelector(`[data-action="${id}"]`)?.classList.toggle("open");
  const b = document.getElementById(id);
  const c = b?.closest(".accordion-trigger")?.nextElementSibling;
  if (c && c.classList.contains("accordion-content")) {
    c.classList.toggle("open");
  }
};
class Suscribe {
  suscribe;
  dialog;
  constructor() {
    const input = document.querySelector(".action-input.suscribe");
    const dialog = document.getElementById("newsletter-success");
    if (input && dialog) {
      this.suscribe = new ActionInput({
        el: input,
        onAction: this.onSuscribe.bind(this)
      });
      this.dialog = new Dialog({
        el: dialog
      });
    }
  }
  onSuscribe(e) {
    e?.preventDefault();
    this.suscribe?.button?.load();
    api.post(ApiRoutes.Newsletters.Suscribe, { email: this.suscribe?.value }).then(() => {
      this.dialog?.open();
    }).catch((error) => {
      console.error(error);
    }).finally(() => this.suscribe?.button?.ready());
  }
}
new Suscribe();
