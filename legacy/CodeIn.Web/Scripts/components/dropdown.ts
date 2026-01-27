import { Dialog, DialogResult } from "./dialog";

export class Dropdown extends Dialog
{
    constructor(props: {
        el: HTMLDialogElement,
        beforeOpen?: () => void,
        afterOpen?: () => void,
        onResult?: (result: DialogResult) => void
    }) {
        super(props);
    }

    protected showAt(position: {x: number, y: number}) {

        if (this.beforeOpen) this.beforeOpen();
        this.el.showModal();

        const dropdownRect = this.el.getBoundingClientRect();
        const totalWidth = document.documentElement.scrollWidth;
        const left = totalWidth- (totalWidth - position.x) - dropdownRect.width;

        this.el.style.left = `${left}px`;
        this.el.style.top = `${position.y}px`;

        if (this.afterOpen) this.afterOpen();
    }
}