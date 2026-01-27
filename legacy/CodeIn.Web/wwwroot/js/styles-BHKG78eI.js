class Component {
  el;
  constructor(props) {
    this.el = props.el;
  }
  hide() {
    this.el.classList.add("hide");
  }
  show() {
    this.el.classList.remove("hide");
  }
}
class Toggler extends Component {
  classes;
  preventDefault;
  onToggleCallback;
  constructor(props) {
    super(props);
    this.preventDefault = !!props.preventDefault;
    this.onToggleCallback = props.onToggle;
    this.classes = Array.from(new Set((props.classes || []).filter((c) => !!c)));
    this.el.addEventListener("click", this.toogle.bind(this));
  }
  toogle(e) {
    this.preventDefault && e.preventDefault();
    this.classes.forEach((c) => this.el.classList.toggle(c));
    this.onToggleCallback && this.onToggleCallback();
  }
}
class Tools {
  static FindParentByClassName(el, className, includeSelf = true) {
    if (className) {
      if (includeSelf && el.classList.contains(className)) return el;
      else if (el.parentElement) return this.FindParentByClassName(el.parentElement, className, true);
    }
    return void 0;
  }
  static FindParentById(el, id, includeSelf = true) {
    if (id) {
      if (includeSelf && el.id === id) return el;
      else if (el.parentElement) return this.FindParentById(el.parentElement, id, true);
    }
    return void 0;
  }
  static FindPreviousSiblingByClass(el, className, avoidClassName, root) {
    var previous = el.previousElementSibling || el.parentElement?.lastElementChild || root || el;
    if (previous === root) return previous;
    else if (!avoidClassName && previous.classList.contains(className)) return previous;
    else if (avoidClassName && !previous.classList.contains(className)) return previous;
    return this.FindPreviousSiblingByClass(previous, className, avoidClassName, root || el);
  }
  static FindNextSiblingByClass(el, className, avoidClassName, root) {
    var next = el.nextElementSibling || el.parentElement?.firstElementChild || root || el;
    if (next === root) return next;
    else if (!avoidClassName && next.classList.contains(className)) return next;
    else if (avoidClassName && !next.classList.contains(className)) return next;
    return this.FindNextSiblingByClass(next, className, avoidClassName, root || el);
  }
  static GetKey(e) {
    let key = "";
    if (e.ctrlKey) key += "ctrl+";
    if (e.altKey) key += "alt+";
    if (e.shiftKey) key += "shift+";
    return key + e.key.toLowerCase();
  }
  static GetCookieByName(cookieName) {
    for (const cookie of document.cookie.split("; ")) {
      const [key, value] = cookie.split("=");
      if (key === cookieName) return decodeURIComponent(value);
    }
  }
}
const AppLanguageKey = "codein-language";
const GetCookieStoredAppLanguage = () => {
  const value = Tools.GetCookieByName(AppLanguageKey)?.slice(-2);
  if (value === "es" || value === "en") return value;
};
const GetAlternative = (lang) => lang === "en" ? "es" : "en";
const StoreCookie = (lang) => document.cookie = `${AppLanguageKey}=c%3D${lang}%7Cuic%3D${lang}; path=/; max-age=31536000`;
const ToggleAppLanguage = () => {
  const value = GetCookieStoredAppLanguage() || "es";
  const alt = GetAlternative(value);
  const classes = document.body.classList;
  classes.remove(value);
  classes.add(alt);
  StoreCookie(alt);
  return alt;
};
const AppThemeKey = "codein-theme";
const MatchScheme = (theme) => window.matchMedia(`(prefers-color-scheme: ${theme})`).matches;
const GetPreferredColorScheme = () => {
  if (MatchScheme("dark")) return "dark";
  if (MatchScheme("light")) return "light";
};
const GetLocalStoredAppTheme = () => {
  const value = localStorage.getItem(AppThemeKey);
  if (value === "dark" || value === "light") return value;
};
const GetCookieStoredAppTheme = () => {
  const value = Tools.GetCookieByName(AppThemeKey);
  if (value === "dark" || value === "light") return value;
};
const GetBodyAppTheme = () => {
  const classes = document.body.classList;
  if (classes.contains("dark")) return "dark";
  if (classes.contains("light")) return "light";
};
const CurrentAppTheme = () => GetCookieStoredAppTheme() || GetBodyAppTheme() || GetPreferredColorScheme() || GetLocalStoredAppTheme() || "light";
const GetAlternativeTheme = (theme) => theme === "dark" ? "light" : "dark";
const StoreLocalTheme = (theme) => localStorage.setItem(AppThemeKey, theme);
const StoreCookieTheme = (theme) => document.cookie = `${AppThemeKey}=${theme}; path=/; max-age=39536000`;
const ToggleAppTheme = () => {
  const value = CurrentAppTheme();
  const alt = GetAlternativeTheme(value);
  const classes = document.body.classList;
  classes.remove(value);
  classes.add(alt);
  StoreLocalTheme(alt);
  StoreCookieTheme(alt);
};
class Header {
  constructor() {
    const theme = document.getElementById("toggle-theme");
    if (theme) {
      new Toggler({
        el: document.getElementById("toggle-theme"),
        onToggle: ToggleAppTheme
      });
    }
    const lang = document.getElementById("toggle-lang");
    if (lang) {
      new Toggler({
        el: document.getElementById("toggle-lang"),
        onToggle: () => {
          ToggleAppLanguage();
          window.location.reload();
        }
      });
    }
    const menu = document.querySelector(".menu-toggle");
    if (menu) {
      new Toggler({
        el: menu,
        onToggle: () => {
          document.body.classList.toggle("open");
        }
      });
    }
  }
}
new Header();
export {
  Component as C
};
