(window as any).showMore = (id: string) => {
    [...document.querySelectorAll(`[data-hide="${id}"]`)].forEach(x => x.classList.remove("hide"))
    document.getElementById(id)?.remove();
}

(window as any).toggleAccordion = (id: string) => {
    document.querySelector(`[data-action="${id}"]`)?.classList.toggle("open");
    const b = document.getElementById(id);
    const c = b?.closest(".accordion-trigger")?.nextElementSibling;
    if (c && c.classList.contains('accordion-content'))
    {
        c.classList.toggle("open");
    }
}