import { Button } from "@/components/button";
import { Component } from "@/components/component";
import { Select } from "@/components/select";

export class ActionSelect extends Component<HTMLElement> {

    public readonly select: Select;
    public readonly button?: Button;

    public get value(): string { return this.select.el.value }
    
    constructor(props: {
        el: HTMLElement,
        default?: string,
        button?: HTMLButtonElement,
        onFocus?: (e: Event) => void,
        onChange?: (e: Event) => void,
        onBlur?: (e: Event) => void,
        onKeyDown?: (e: KeyboardEvent) => void,
        onAction?: (e: Event) => void
    }) {

        super({el: props.el});

        this.clearErrors = this.clearErrors.bind(this);

        this.select = new Select({
            ...props, 
            el: this.el.querySelector(":scope select") as HTMLSelectElement,
            onChange: (e: Event) => 
            {
                this.clearErrors();
                if (props.onChange) props.onChange(e);
            }
        });

        const button = props.button || this.el.querySelector(":scope button") as HTMLButtonElement
        if (button)
        {
            this.button = new Button({
                el: button,
                onClick: props.onAction
            });
        }

    }

    public setValue(value?: string) {
        this.select.setValue(value);
    }

    public clear() {
        this.select.clear();
        this.clearErrors();
    }

    public clearErrors() {
        document.querySelectorAll(":scope .error").forEach(e => e.setAttribute("hidden", "true"));
    }

    public setError(type: string) {
        this.el.querySelector(`:scope .error[data-type="${type}"]`)?.removeAttribute("hidden");
    }

}