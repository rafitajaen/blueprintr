import '@/shared/header';
import '@styles/styles.scss';

class MagicLink
{
    constructor()
    {
        window.location.href = document.querySelector<HTMLElement>("section.magic-link")?.dataset.redirectUrl || "/";
    }
}

new MagicLink();