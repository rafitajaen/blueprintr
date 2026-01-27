import { Highlighter } from "../composables/highlighter";
import { Tools } from "../composables/tools";
import { Input } from "./input";

export class Autocomplete
{
    public el: HTMLElement;

    public input: Input;
    public ul: HTMLUListElement;
    public lis: HTMLLIElement[];

    public get selected(): HTMLElement | undefined { return this.lis.find(li => li.classList.contains("hovered")) }
    public get isOpened(): boolean { return !this.ul.classList.contains("hide") }

    constructor(props: {
        el: HTMLElement
    }) {
        this.el = props.el;

        this.ul = this.el.querySelector(":scope > ul") as HTMLUListElement;
        this.lis = [...this.ul.querySelectorAll<HTMLLIElement>(":scope > li")];

        this.input = new Input({
            el: this.el.querySelector(":scope > input")!,
            onFocus: this.onFocus.bind(this),
            onInput: this.onInput.bind(this),
            onKeyDown: this.onKeyDown.bind(this),
            // onBlur: this.onBlur.bind(this)
        });

        document.addEventListener("click", this.onClickOutside.bind(this));
    }

    onClickOutside(e: Event) {
        if (!this.isOpened || !e || !e.target) return;
        const el = Tools.FindParentByClassName(e.target as HTMLElement, "autocomplete");
        if (!el || el !== this.el) this.onEnter();
    }

    onKeyDown(e: KeyboardEvent) {

        switch (Tools.GetKey(e)) {
            case "enter":
                this.onEnter();
                break;
            case "escape": 
                this.close();
                break;
            case "arrowup":
                this.selectItem(Tools.FindPreviousSiblingByClass((this.selected || this.ul.firstElementChild) as HTMLElement, "hide", true));
                break;
            case "arrowdown":
                this.selectItem(Tools.FindNextSiblingByClass((this.selected || this.ul.lastElementChild) as HTMLElement, "hide", true));
                break;

        }
    }

    onFocus(e: Event) {

        e.preventDefault();
        e.stopPropagation();

        this.ul.addEventListener("click", this.onClick);
        this.lis.forEach(li => {
            li.addEventListener("mouseenter", this.onMouseEnter);
            li.addEventListener("mouseleave", this.onMouseLeave);
        });

        if (this.input.el.dataset.code?.length) {
            this.selectItem(this.lis.find(li => li.dataset.code === this.input.el.dataset.code));
        }

        this.reloadItems();
        this.show();
    }

    onInput() {
        this.reloadItems();
    }

    onEnter() {
        this.fillWith(this.selected);
        this.close();
    }

    onClick = (e: Event) => {
        if (!e || !e.target) return;
        this.fillWith(Tools.FindParentByClassName(e.target as HTMLElement, "item"))
        this.close();
    }

    onMouseEnter = (e: Event) => {
        if (!e || !e.target) return;
        this.selectItem(e.target as HTMLElement);
    }

    onMouseLeave = (e: Event) => {
        if (!e || !e.target) return;
        (e.target as HTMLElement).classList.remove("hovered");
    }

    hide() { this.ul.classList.add("hide"); }

    show() { this.ul.classList.remove("hide") }

    selectItem(el?: HTMLElement) {
        if (!el) return;
        if (this.selected) this.selected.classList.remove("hovered");
        el.classList.add("hovered");
        el.scrollIntoView({behavior: "auto", block: "nearest", inline: "nearest"});
        this.fillWith(el);
    }

    private fillWith(item?: HTMLElement) {
        this.input.el.value = item?.innerText || this.input.default;
        this.input.el.dataset.code = item?.dataset?.code || "";
    }

    private reloadItems() {
        const search = this.input.el.value.trim();
        Highlighter.Elements(search, ...this.lis);

        this.lis.forEach(li => {
            if (!search || li.innerHTML.includes("</mark>")) li.classList.remove("hide");
            else li.classList.add("hide");
        });
    }

    private close() {

        console.trace()
        this.input.el.blur();
        this.hide();
        this.ul.removeEventListener("click", this.onClick);
        this.lis.forEach(li => {
            li.removeEventListener("mouseenter", this.onMouseEnter);
            li.removeEventListener("mouseleave", this.onMouseLeave);
        });
        if (this.selected) this.selected.classList.remove("hovered");
    }

}