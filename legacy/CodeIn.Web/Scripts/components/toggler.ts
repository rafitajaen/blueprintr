import { Component } from "./component";

export class Toggler extends Component<HTMLElement>
{
    protected classes: string[];
    protected preventDefault: boolean;
    protected onToggleCallback?: () => void;
    
    constructor(props: 
    {
        el: HTMLElement,
        classes?: string[],
        preventDefault?: boolean
        onToggle?: () => void;
    })
    {
        super(props);
        this.preventDefault = !!props.preventDefault;
        this.onToggleCallback = props.onToggle;
        this.classes = Array.from(new Set((props.classes || []).filter(c => !!c)));
        this.el.addEventListener("click", this.toogle.bind(this));
    }

    private toogle(e: MouseEvent)
    {
        this.preventDefault && e.preventDefault();
        this.classes.forEach(c => this.el.classList.toggle(c));
        this.onToggleCallback && this.onToggleCallback();
    }
}