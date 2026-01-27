import "./styles-BHKG78eI.js";
import "./empty.js";
import { B as Button } from "./dialog-B8FD7tRf.js";
class Jobs {
  showFilters;
  apply;
  applyFilters;
  constructor() {
    this.showFilters = new Button({
      el: document.getElementById("show-filters"),
      onClick: this.onShowFilters.bind(this)
    });
    this.apply = new Button({
      el: document.getElementById("apply"),
      onClick: this.onApplyFilters.bind(this)
    });
    this.applyFilters = new Button({
      el: document.getElementById("apply-filters"),
      onClick: this.onApplyFilters.bind(this)
    });
  }
  onShowFilters() {
    document.querySelector("aside.filters")?.classList.toggle("open");
  }
  onApplyFilters() {
    const kinds = this.getKinds();
    const url = new URL(window.location.href);
    kinds.forEach((kind2) => {
      const selected = this.getCheckedOfKind(kind2);
      if (selected.length > 0) url.searchParams.set(kind2, selected.join("."));
      else url.searchParams.delete(kind2);
    });
    const kind = "Salary";
    const salary = document.getElementById("salaries").value;
    if (salary && salary !== "From0k") url.searchParams.set(kind, salary);
    else url.searchParams.delete(kind);
    window.location.href = url.toString();
  }
  // private onCheckboxChange(e: Event)
  // {
  //     if (!e?.target) return;
  //     const target = e.target as HTMLInputElement;
  //     const kind = target.dataset.kind;
  //     if (!kind) return;
  //     const url = new URL(window.location.href);
  //     const selected = this.getCheckedOfKind(kind);
  //     if (selected.length > 0) {
  //         url.searchParams.set(kind, selected.join('.'));
  //     } else {
  //         url.searchParams.delete(kind);
  //     }
  //     window.location.href = url.toString();
  // }
  getCheckedOfKind(kind) {
    const names = [];
    const checkboxes = document.querySelectorAll(`input[type="checkbox"][data-kind="${kind}"]`);
    checkboxes.forEach((x) => {
      if (x instanceof HTMLInputElement && x.checked && x.name) {
        names.push(x.name);
      }
    });
    return names;
  }
  getKinds() {
    const elements = document.querySelectorAll('input[type="checkbox"][data-kind]');
    const kinds = /* @__PURE__ */ new Set();
    elements.forEach((el) => {
      const kind = el.getAttribute("data-kind");
      if (kind) kinds.add(kind);
    });
    return Array.from(kinds);
  }
}
new Jobs();
