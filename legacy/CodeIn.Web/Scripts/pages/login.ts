import '@/shared/header';
import '@styles/styles.scss';

import { api } from '@/axios/api';
import { ApiRoutes } from '@/axios/api-routes';
import { Dialog } from '@/components/dialog';
import { AxiosError } from 'axios';
import { ActionInput } from '@/components/action-input';
import { PageRoutes } from '@/axios/page-routes';

class Login
{
    public email: ActionInput;
    private failed: Dialog;
    
    constructor() {

        this.email = new ActionInput({
            el: document.querySelector(".email-action") as HTMLElement,
            button: document.getElementById("submit") as HTMLButtonElement,
            onAction: this.onSubmit.bind(this)
        });

        this.failed = new Dialog({
            el: document.getElementById("login-failed") as HTMLDialogElement
        });

    }

    private onSubmit(e?: Event) {

        e?.preventDefault();

        this.email.button?.load();

        const parsed = new URL(window.location.href);
        const email = this.email.value;

        api
        .post(ApiRoutes.Auth.Login, { 
            email: email,
            returnUrl:  parsed.searchParams.get("returnUrl") || undefined,
            username: parsed.searchParams.get("username") || undefined,
        })
        .then(() => {
            window.location.href = `${PageRoutes.Auth.Verify}?email=${email}`
        })
        .catch((error: AxiosError) => {

            if (error.response?.status === 400)
            {
                this.email.setError("Email.Invalid");
                return;
            }

            this.failed.open();
        })
        .finally(() => this.email.button?.ready());
    }

}

new Login();
