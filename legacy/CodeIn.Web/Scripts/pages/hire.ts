import '@/shared/header';
import '@/shared/shared';
import '@/shared/suscribe';
import '@styles/styles.scss';

import { api } from '@/axios/api';
import { ApiRoutes } from '@/axios/api-routes';
import { ActionArea } from '@/components/action-area';
import { ActionInput } from '@/components/action-input';
import { ActionSelect } from '@/components/action-select';
import { Button } from '@/components/button';
import { ProblemDetails } from '@/composables/problem-details';
import { AxiosError } from 'axios';
import { PageRoutes } from '@/axios/page-routes';
import { Dialog } from '@/components/dialog';

class Hire {
    // Company
    public company: ActionInput;
    public hq: ActionInput;
    public website: ActionInput;

    // Job
    public role: ActionInput;
    public location: ActionInput;
    public arrangement: ActionSelect;
    public salary: ActionSelect;
    public description: ActionArea;

    // Contact person
    public name: ActionInput;
    public email: ActionInput;

    // Submit button
    public submit: Button;

    // Dialog
    private failed: Dialog;

    constructor() {
        // Company fields
        this.company = new ActionInput({
            el: document.querySelector(`[data-input-id="company"]`) as HTMLElement
        });

        this.hq = new ActionInput({
            el: document.querySelector(`[data-input-id="hq"]`) as HTMLElement
        });

        this.website = new ActionInput({
            el: document.querySelector(`[data-input-id="website"]`) as HTMLElement
        });

        // Job fields
        this.role = new ActionInput({
            el: document.querySelector(`[data-input-id="role"]`) as HTMLElement
        });

        this.location = new ActionInput({
            el: document.querySelector(`[data-input-id="location"]`) as HTMLElement
        });

        this.arrangement = new ActionSelect({
            el: document.querySelector(`[data-input-id="arrangement"]`) as HTMLElement
        });

        this.salary = new ActionSelect({
            el: document.querySelector(`[data-input-id="salary"]`) as HTMLElement
        });

        this.description = new ActionArea({
            el: document.querySelector(`[data-input-id="description"]`) as HTMLElement
        });

        // Contact fields
        this.name = new ActionInput({
            el: document.querySelector(`[data-input-id="name"]`) as HTMLElement
        });

        this.email = new ActionInput({
            el: document.querySelector(`[data-input-id="email"]`) as HTMLElement
        });

        // Submit button
        this.submit = new Button({
            el: document.getElementById("submit") as HTMLButtonElement,
            onClick: this.onSubmit.bind(this)
        });

        // Dialog
        this.failed = new Dialog({
            el: document.getElementById("submit-failed") as HTMLDialogElement
        });
    }

    private onSubmit(e?: Event): void {

        e?.preventDefault();

        if (!this.validate()) return;

        this.submit.load();

        const data = {
            company: this.company.value,
            hq: this.hq.value,
            website: this.website.value,
            role: this.role.value,
            location: this.location.value,
            arrangement: this.arrangement.value,
            salary: this.salary.value,
            description: this.description.value,
            name: this.name.value,
            email: this.email.value
        };

        api
        .post(ApiRoutes.Leads.Submissions, data)
        .then(() => {
            window.location.href = `${PageRoutes.Auth.Verify}?email=${data.email}`
        })
        .catch((error: AxiosError) => {

            console.log({error})
            const problems = new ProblemDetails(error);
            if (problems.contains("EmailValidator", "Email")) this.email.setError("InvalidEmail");
            if (problems.containsTitle("CompanySubmission.Email.AlreadyInUse")) this.email.setError("AlreadyInUse");
            if (problems.containsTitle("CompanySubmission.UnavailableService")) this.failed.open();
        })
        .finally(() => this.submit.ready());
    }

    private validate(): boolean {
        let hasErrors = this.company.validate();
        hasErrors = this.hq.validate() || hasErrors;
        hasErrors = this.website.validate() || hasErrors;
        hasErrors = this.role.validate() || hasErrors;
        hasErrors = this.location.validate() || hasErrors;
        hasErrors = this.description.validate() || hasErrors;
        hasErrors = this.name.validate() || hasErrors;
        hasErrors = this.email.validate() || hasErrors;

        return !hasErrors;
    }
}

new Hire();
