import { Toggler } from "../components/toggler";
import { ToggleAppLanguage } from "../composables/app-language";
import { ToggleAppTheme } from "../composables/app-theme";

class Header
{
    constructor() {

        // Theme Toggler
        const theme = document.getElementById("toggle-theme") as HTMLElement;
        if (theme)
        {
            new Toggler({
                el: document.getElementById("toggle-theme") as HTMLElement,
                onToggle: ToggleAppTheme
            });
        }

        const lang = document.getElementById("toggle-lang") as HTMLElement;
        if (lang)
        {
            new Toggler({
                el: document.getElementById("toggle-lang") as HTMLElement,
                onToggle: () => 
                {
                    ToggleAppLanguage();
                    window.location.reload();
                }
            });
        }

        // Menu Toggler
        const menu = document.querySelector(".menu-toggle") as HTMLElement;
        if (menu)
        {
            new Toggler({
                el: menu,
                onToggle: () => { document.body.classList.toggle("open") }
            });
        }
            
    }

}

export const header = new Header();