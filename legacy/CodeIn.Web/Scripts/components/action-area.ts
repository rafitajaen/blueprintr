import { Button } from "@/components/button";
import { Component } from "@/components/component";
import { TextArea } from "@/components/textarea";

export class ActionArea extends Component<HTMLElement> {

    public readonly textarea: TextArea;
    public readonly button?: Button;

    public get value(): string { return this.textarea.el.value }
    public get required(): boolean { return this.el.dataset.required?.toLowerCase() === "true" }
    
    constructor(props: {
        el: HTMLElement,
        default?: string,
        button?: HTMLButtonElement,
        onFocus?: (e: Event) => void,
        onInput?: (e: Event) => void,
        onBlur?: (e: Event) => void,
        onKeyDown?: (e: KeyboardEvent) => void,
        onAction?: (e: Event) => void
    }) {

        super({el: props.el});

        this.clearErrors = this.clearErrors.bind(this);

        this.textarea = new TextArea({
            ...props, 
            el: this.el.querySelector(":scope textarea") as HTMLTextAreaElement,
            onInput: (e: Event) => 
            {
                this.clearErrors();
                if (props.onInput) props.onInput(e);
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
        this.textarea.setValue(value);
    }

    public clear() {
        this.textarea.clear();
        this.clearErrors();
    }

    public clearErrors() {
        document.querySelectorAll(":scope .error").forEach(e => e.setAttribute("hidden", "true"));
    }

    public setError(type: string) {
        this.el.querySelector(`:scope .error[data-type="${type}"]`)?.removeAttribute("hidden");
        this.textarea.el.scrollIntoView({behavior: "auto", block: "center", inline: "center"});
    }

    public validate(): boolean {
        const hasErrors = this.required && !this.value;
        if (hasErrors) this.setError("Empty");
        return hasErrors;
    }

}