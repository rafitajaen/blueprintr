import "./styles-BHKG78eI.js";
class MagicLink {
  constructor() {
    window.location.href = document.querySelector("section.magic-link")?.dataset.redirectUrl || "/";
  }
}
new MagicLink();
