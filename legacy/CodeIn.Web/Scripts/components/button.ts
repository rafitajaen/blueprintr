import { Component } from "./component";

export class Button extends Component<HTMLElement> {

    constructor(props: {
        el: HTMLElement,
        onClick?: (e: Event) => void,
        onMouseUp?: (e?: Event) => void,
        onMouseDown?: (e?: Event) => void,
    }) {

        super(props);
        if (props.onClick) this.el.addEventListener("click", props.onClick);
        if (props.onMouseUp) this.el.addEventListener("mouseup", props.onMouseUp);
        if (props.onMouseDown) this.el.addEventListener("mousedown", props.onMouseDown);
    }

    public load() {
        this.el.classList.add("button-loading");
    }

    public ready() {
        this.el.classList.remove("button-loading");
    }
}