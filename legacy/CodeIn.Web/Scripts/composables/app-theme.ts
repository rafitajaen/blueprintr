import { Tools } from "./tools";

/**
 * Defines the possible values for the application theme.
 * 
 * The `AppTheme` type defines the two valid theme values for the application: 
 * - `'dark'` for dark mode.
 * - `'light'` for light mode.
 * 
 * This type is used throughout the application to ensure that only valid theme values are used.
 * 
 * @type {AppTheme}
 * 
 * @example
 * // Example usage:
 * const currentTheme: AppTheme = "dark"; // Valid
 * const currentTheme2: AppTheme = "light"; // Valid
 * const currentTheme3: AppTheme = "blue"; // Error: Type '"blue"' is not assignable to type 'AppTheme'.
 */
export type AppTheme = "dark" | "light";

/**
 * The key used to store the theme preference in localStorage.
 * 
 * The `AppThemeKey` constant holds the string that is used as the key to save and retrieve
 * the user's selected theme in `localStorage`. This key is used consistently throughout
 * the application to ensure that the theme preference is correctly saved and accessed.
 * 
 * @constant
 * @type {string}
 * 
 * @example
 * // Example usage:
 * localStorage.setItem(AppThemeKey, "dark"); // Store 'dark' theme preference
 * const theme = localStorage.getItem(AppThemeKey); // Retrieve theme preference
 */
const AppThemeKey: string = import.meta.env.VITE_THEME_COOKIE || "codein-theme";

/**
 * Checks if the user's preferred color scheme matches the specified theme.
 * 
 * @param theme - The desired color scheme to check against, which can be 'light' or 'dark'.
 * @returns {boolean} - Returns `true` if the window's preferred color scheme matches the provided theme ('light' or 'dark'), otherwise `false`.
 * 
 * @example
 * // Example usage:
 * const isDarkMode = MatchScheme('dark');
 * console.log(isDarkMode); // true if the user's system is set to dark mode
 */
const MatchScheme = (theme: AppTheme): boolean => window.matchMedia(`(prefers-color-scheme: ${theme})`).matches

/**
 * Retrieves the user's preferred color scheme based on the system's preference.
 * 
 * This function checks the system's color scheme preference by calling the `MatchScheme` function
 * with both 'dark' and 'light' values. It returns the user's preferred theme if one is detected.
 * If the system preference cannot be determined, it returns `undefined`.
 * 
 * @returns {AppTheme | undefined} - Returns:
 *   - `'dark'` if the system's color scheme is dark.
 *   - `'light'` if the system's color scheme is light.
 *   - `undefined` if no preference is detected or the system's preference is unknown.
 * 
 * @example
 * // Example usage:
 * const preferredTheme = GetPreferredColorScheme();
 * console.log(preferredTheme); // 'dark', 'light', or undefined
 */
const GetPreferredColorScheme = (): AppTheme | undefined => {
    if (MatchScheme("dark")) return 'dark';
    if (MatchScheme("light")) return 'light';
}

/**
 * Retrieves the stored application theme from localStorage.
 * 
 * This function checks if a theme (either 'dark' or 'light') is stored in the localStorage under the key `AppThemeKey`.
 * If a valid theme value is found, it returns the stored theme. If no valid value is found or the key does not exist,
 * it returns `undefined`.
 * 
 * @returns {AppTheme | undefined} - Returns:
 *   - `'dark'` if the stored theme in localStorage is 'dark'.
 *   - `'light'` if the stored theme in localStorage is 'light'.
 *   - `undefined` if the theme is not set in localStorage or the value is invalid.
 * 
 * @example
 * // Example usage:
 * const storedTheme = GetStoredAppTheme();
 * console.log(storedTheme); // 'dark', 'light', or undefined
 */
const GetLocalStoredAppTheme = (): AppTheme | undefined => {
    const value = localStorage.getItem(AppThemeKey);
    if (value === "dark" || value === "light") return value;
}

const GetCookieStoredAppTheme = (): AppTheme | undefined => {
    const value = Tools.GetCookieByName(AppThemeKey);
    if (value === "dark" || value === "light") return value;
}

/**
 * Retrieves the current application theme based on the body's class.
 * 
 * This function checks the `classList` of the `body` element for the presence of the `dark` or `light` class.
 * If a valid class is found, it returns the corresponding theme. If no valid class is found, it returns `undefined`.
 * 
 * @returns {AppTheme | undefined} - Returns:
 *   - `'dark'` if the body element contains the class 'dark'.
 *   - `'light'` if the body element contains the class 'light'.
 *   - `undefined` if neither class is present in the body element.
 * 
 * @example
 * // Example usage:
 * const bodyTheme = GetBodyAppTheme();
 * console.log(bodyTheme); // 'dark', 'light', or undefined
 */
const GetBodyAppTheme = (): AppTheme | undefined => {
    const classes = document.body.classList;
    if (classes.contains("dark")) return "dark";
    if (classes.contains("light")) return "light";
}

/**
 * Retrieves the current application theme by checking multiple sources in order of priority.
 * 
 * This function checks the stored theme in `localStorage`, the theme applied to the `body` element through its class,
 * and the user's preferred color scheme based on system settings (via `MatchScheme`).
 * If none of these sources provide a theme, it defaults to `'light'`.
 * 
 * @returns {AppTheme} - Returns the current application theme:
 *   - `'dark'` if a valid 'dark' theme is found.
 *   - `'light'` if a valid 'light' theme is found.
 *   - If no valid theme is found, it defaults to `'light'`.
 * 
 * @example
 * // Example usage:
 * const currentTheme = CurrentAppTheme();
 * console.log(currentTheme); // 'dark' or 'light'
 */
const CurrentAppTheme = (): AppTheme => GetCookieStoredAppTheme() || GetBodyAppTheme() || GetPreferredColorScheme() || GetLocalStoredAppTheme() || "light";

/**
 * Retrieves the alternative theme to the provided one.
 * 
 * This function takes the current theme (`'dark'` or `'light'`) and returns the opposite theme.
 * If the current theme is `'dark'`, it returns `'light'`, and if the current theme is `'light'`, it returns `'dark'`.
 * 
 * @param theme {AppTheme} - The current theme, either `'dark'` or `'light'`.
 * @returns {AppTheme} - Returns the alternative theme:
 *   - If the input theme is `'dark'`, it returns `'light'`.
 *   - If the input theme is `'light'`, it returns `'dark'`.
 * 
 * @example
 * // Example usage:
 * const alternativeTheme = GetAlternativeTheme('dark');
 * console.log(alternativeTheme); // 'light'
 * 
 * const alternativeTheme2 = GetAlternativeTheme('light');
 * console.log(alternativeTheme2); // 'dark'
 */
const GetAlternativeTheme = (theme: AppTheme): AppTheme =>  theme === "dark" ? "light" : "dark";

const StoreLocalTheme = (theme: AppTheme): void => localStorage.setItem(AppThemeKey, theme);
const StoreCookieTheme = (theme: AppTheme): string => document.cookie = `${AppThemeKey}=${theme}; path=/; max-age=39536000`;

/**
 * Toggles the current application theme between 'dark' and 'light'.
 * 
 * This function retrieves the current theme using the `CurrentAppTheme` function and then
 * toggles it by switching to the alternative theme (i.e., from 'dark' to 'light' or vice versa).
 * The new theme is stored in `localStorage` under the key `AppThemeKey`.
 * 
 * @returns {void} - This function does not return any value.
 * 
 * @example
 * // Example usage:
 * ToogleAppTheme(); // This will toggle the theme between 'dark' and 'light' and store the new value in localStorage.
 */
export const ToggleAppTheme = (): void => {

    const value = CurrentAppTheme();
    const alt = GetAlternativeTheme(value);

    const classes = document.body.classList;
    classes.remove(value);
    classes.add(alt);

    StoreLocalTheme(alt);
    StoreCookieTheme(alt);
} 
  