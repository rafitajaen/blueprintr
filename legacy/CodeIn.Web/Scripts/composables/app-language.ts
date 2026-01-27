import { Tools } from "./tools";

export type AppLanguage = "es" | "en";
const AppLanguageKey: string = import.meta.env.VITE_LANGUAGE_COOKIE || "codein-language";

const GetCookieStoredAppLanguage = (): AppLanguage | undefined => {
    const value = Tools.GetCookieByName(AppLanguageKey)?.slice(-2);
    if (value === "es" || value === "en") return value;
}

const GetAlternative = (lang: AppLanguage | undefined): AppLanguage =>  lang === "en" ? "es" : "en";

const StoreCookie = (lang: AppLanguage): string => document.cookie = `${AppLanguageKey}=c%3D${lang}%7Cuic%3D${lang}; path=/; max-age=31536000`;

export const ToggleAppLanguage = (): string => {

    const value = GetCookieStoredAppLanguage() || "es";
    const alt = GetAlternative(value);

    const classes = document.body.classList;
    classes.remove(value);
    classes.add(alt);

    StoreCookie(alt);

    return alt;
} 
  