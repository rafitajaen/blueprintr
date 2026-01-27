import { Component } from "./component";

export class Input extends Component<HTMLInputElement>
{
    public readonly default: string;
    
    constructor(props: {
        el: HTMLInputElement,
        default?: string,
        onFocus?: (e: Event) => void,
        onChange?: (e: Event) => void,
        onInput?: (e: Event) => void,
        onBlur?: (e: Event) => void,
        onKeyDown?: (e: KeyboardEvent) => void
    }) {

        super(props);

        this.default = props.default || "";

        if (props.onFocus) this.el.addEventListener("focus", props.onFocus);
        if (props.onChange) this.el.addEventListener("change", props.onChange);
        if (props.onInput) this.el.addEventListener("input", props.onInput);
        if (props.onBlur) this.el.addEventListener("blur", props.onBlur);
        if (props.onKeyDown) this.el.addEventListener("keydown", props.onKeyDown);
    }

    public clear() {
        this.el.value = this.default;
    }

    public setValue(value?: string) {
        this.el.value = value === undefined ? this.default : value;
    }
}