export class Tools
{
    public static FindParentByClassName(el: HTMLElement, className: string, includeSelf = true): HTMLElement | undefined
    {
        if (className)
        {
            if (includeSelf && el.classList.contains(className)) return el;
            else if (el.parentElement) return this.FindParentByClassName(el.parentElement, className, true);
        }

        return undefined;
    }

    public static FindParentById(el: HTMLElement, id: string, includeSelf = true): HTMLElement | undefined
    {
        if (id)
        {
            if (includeSelf && el.id === id) return el;
            else if (el.parentElement) return this.FindParentById(el.parentElement, id, true);
        }

        return undefined;
    }

    public static FindPreviousSiblingByClass(el: HTMLElement, className: string, avoidClassName: boolean, root?: HTMLElement): HTMLElement {
        var previous = (el.previousElementSibling || el.parentElement?.lastElementChild || root || el) as HTMLElement;

        if (previous === root) return previous;
        else if (!avoidClassName && previous.classList.contains(className)) return previous;
        else if (avoidClassName && !previous.classList.contains(className)) return previous;
        return this.FindPreviousSiblingByClass(previous, className, avoidClassName, root || el);
    }

    public static FindNextSiblingByClass(el: HTMLElement, className: string, avoidClassName: boolean, root?: HTMLElement): HTMLElement {
        var next = (el.nextElementSibling || el.parentElement?.firstElementChild || root || el) as HTMLElement;

        if (next === root) return next;
        else if (!avoidClassName && next.classList.contains(className)) return next;
        else if (avoidClassName && !next.classList.contains(className)) return next;
        return this.FindNextSiblingByClass(next, className, avoidClassName, root || el);
    }

    public static GetKey(e: KeyboardEvent)
    {
        let key = "";
        if (e.ctrlKey) key += "ctrl+";
        if (e.altKey) key += "alt+";
        if (e.shiftKey) key += "shift+";
        return key + e.key.toLowerCase();
    }

    public static GetCookieByName(cookieName: string): string | undefined
    {
        for (const cookie of document.cookie.split('; ')) {
            const [key, value] = cookie.split('=');
            if (key === cookieName) return decodeURIComponent(value);
        }
    }
}