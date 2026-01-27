import { Component } from "./component";

export class Select extends Component<HTMLSelectElement>
{
    public readonly default: string;
    
    constructor(props: {
        el: HTMLSelectElement,
        default?: string,
        onFocus?: (e: Event) => void,
        onChange?: (e: Event) => void,
        onBlur?: (e: Event) => void,
        onKeyDown?: (e: KeyboardEvent) => void
    }) {

        super(props);

        this.default = props.default || "";

        if (props.onFocus) this.el.addEventListener("focus", props.onFocus);
        if (props.onChange) this.el.addEventListener("change", props.onChange);
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