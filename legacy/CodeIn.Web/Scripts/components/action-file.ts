import { Button } from "@/components/button";
import { Component } from "@/components/component";
import { Input } from "@/components/input";

export class ActionFile extends Component<HTMLElement> {

    public readonly input: Input;
    public readonly button?: Button;

    private _value: string = "";
    public get value(): string { return this._value }
    public get required(): boolean { return this.el.dataset.required?.toLowerCase() === "true" }
    
    constructor(props: {
        el: HTMLElement,
        default?: string,
        button?: HTMLButtonElement,
        onFocus?: (e: Event) => void,
        onChange?: (e: Event) => void,
        onInput?: (e: Event) => void,
        onBlur?: (e: Event) => void,
        onKeyDown?: (e: KeyboardEvent) => void,
        onAction?: (e: Event) => void
    }) {

        super({el: props.el});

        this.clearErrors = this.clearErrors.bind(this);
        this.onInputChange = this.onInputChange.bind(this);

        this.input = new Input({
            ...props, 
            el: this.el.querySelector(":scope input") as HTMLInputElement,
            onInput: (e: Event) => 
            {
                this.clearErrors();
                if (props.onInput) props.onInput(e);
            },
            onChange: (e: Event) => 
            {
                this.onInputChange(e);
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

    private onInputChange(e: Event)
    {
        e.stopPropagation();

        const files = this.input.el.files;
        if (files && files.length > 0)
        {
            const file = files[0];
            if (file)
            {
                const reader = new FileReader();
                reader.onload = (event: any) => {
                    this._value = event.target.result;
                };

                reader.onerror = (error) => {
                    this._value = "";
                    console.error("Error reading file:", error);
                };

                reader.readAsDataURL(file); // <- Conver file to base64
            }
        }
    }

    public setValue(value?: string) {
        this.input.setValue(value);
    }

    public clear() {
        this.input.clear();
        this.clearErrors();
    }

    public clearErrors() {
        document.querySelectorAll(":scope .error").forEach(e => e.setAttribute("hidden", "true"));
    }

    public setError(type: string) {
        this.el.querySelector(`:scope .error[data-type="${type}"]`)?.removeAttribute("hidden");
        this.input.el.scrollIntoView({behavior: "auto", block: "center", inline: "center"});
    }

    public validate(): boolean {
        const hasErrors = this.required && !this.value;
        if (hasErrors) this.setError("Empty");
        return hasErrors;
    }

}