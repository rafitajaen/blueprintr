export abstract class Component<TElement extends HTMLElement> {

    public readonly el: TElement;

    constructor(props: {
        el: TElement
    }) {
        this.el = props.el;
    }

    public hide() { this.el.classList.add("hide") }
    public show() { this.el.classList.remove("hide") }

}