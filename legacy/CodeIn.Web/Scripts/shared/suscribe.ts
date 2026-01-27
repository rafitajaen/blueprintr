import { ActionInput } from '@/components/action-input';
import { api } from '@/axios/api';
import { ApiRoutes } from '@/axios/api-routes';
import { AxiosError, AxiosResponse } from 'axios';
import { Dialog } from '@/components/dialog';

class Suscribe
{
    private suscribe?: ActionInput;
    private dialog?: Dialog;

    constructor() {

        const input = document.querySelector(".action-input.suscribe") as HTMLElement;
        const dialog = document.getElementById("newsletter-success") as HTMLDialogElement;

        if (input && dialog)
        {
            this.suscribe = new ActionInput({
                el: input,
                onAction: this.onSuscribe.bind(this)
            });
    
            this.dialog = new Dialog({
                el: dialog
            });
        }
    }

    private onSuscribe(e?: Event) {

        e?.preventDefault();

        this.suscribe?.button?.load();

        api
        .post(ApiRoutes.Newsletters.Suscribe, { email: this.suscribe?.value })
        .then(() => {
            // console.log(response);
            this.dialog?.open();
        })
        .catch((error: AxiosError) => {
            console.error(error);
        })
        .finally(() => this.suscribe?.button?.ready());
    }

}

new Suscribe();