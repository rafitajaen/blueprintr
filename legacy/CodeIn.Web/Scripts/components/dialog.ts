import { Button } from "./button";

export type DialogResult = "action" | "cancel" | "closed";

export class Dialog
{
    public readonly el: HTMLDialogElement;
    
    protected beforeOpen?: () => void;
    protected afterOpen?: () => void;
    protected onResult?: (result: DialogResult) => void

    protected action?: Button;
    protected cancel?: Button;
    protected close?: Button;

    constructor(props: {
        el: HTMLDialogElement,
        beforeOpen?: () => void,
        afterOpen?: () => void,
        onResult?: (result: DialogResult) => void
    }) {

        this.el = props.el;

        this.beforeOpen = props.beforeOpen || this.beforeOpen;
        this.afterOpen = props.afterOpen || this.afterOpen;
        this.onResult = props.onResult || this.onResult;

        const close = this.el.querySelector(":scope button.close") as HTMLButtonElement;
        if (close) this.close = new Button({
            el: close,
            onClick: this.onClose.bind(this)
        });

        const cancel = this.el.querySelector(":scope button.dialog-cancel") as HTMLButtonElement;
        if (cancel) this.cancel = new Button({
            el: cancel,
            onClick: this.onCancel.bind(this)
        });

        const action = this.el.querySelector(":scope button.dialog-action") as HTMLButtonElement;
        if (action) this.action = new Button({
            el: action,
            onClick: this.onAction.bind(this)
        });
    }

    private onClose(e?: Event) {
        e?.preventDefault();
        if (this.onResult) this.onResult("closed");
        else this.el.close();
    }

    private onCancel(e?: Event) {
        e?.preventDefault();
        if (this.onResult) this.onResult("cancel");
        else this.el.close();
    }

    private onAction(e?: Event) {
        e?.preventDefault();
        if (this.onResult) this.onResult("action");
        else this.el.close();
    }

    public open() {
        if (this.beforeOpen) this.beforeOpen();
        this.el.showModal();
        if (this.afterOpen) this.afterOpen();
    }

}