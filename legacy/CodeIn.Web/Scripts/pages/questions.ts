import '@/shared/header';
import '@/shared/shared';
import '@/shared/suscribe';
import '@styles/styles.scss';

import { api } from '@/axios/api';
import { ApiRoutes } from '@/axios/api-routes';
import { ActionArea } from '@/components/action-area';
import { ActionInput } from '@/components/action-input';
import { Button } from '@/components/button';
import { ProblemDetails } from '@/composables/problem-details';
import { AxiosError } from 'axios';
import { Dialog } from '@/components/dialog';

class JobQuestion {

    // Fields
    public email: ActionInput;
    public description: ActionArea;

    // Submit button
    public submit: Button;

    // Dialog
    private failed: Dialog;

    public get jobId(): string | undefined { return document.querySelector("article")?.dataset.jobId }

    constructor() {

        // Fields
        this.description = new ActionArea({
            el: document.querySelector(`[data-input-id="description"]`) as HTMLElement
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
            jobId: this.jobId,
            question: this.description.value,
            email: this.email.value,
        };

        api
        .post(ApiRoutes.Questions.Submissions, data)
        .then(() => {
            const url = new URL(window.location.href);
            url.pathname = url.pathname.replace(/\/$/, '') + "/submitted";
            if (data.email) url.searchParams.append("email", data.email);
            window.location.href = url.toString();
        })
        .catch((error: AxiosError) => {

            console.log({error})
            const problems = new ProblemDetails(error);
            if (problems.contains("EmailValidator", "Email")) this.email.setError("InvalidEmail");
            if (problems.contains("MaximumLengthValidator", "Question")) this.description.setError("TooLong");
            if (problems.containsTitle("QuestionSubmission.Job.Id")) this.failed.open();
        })
        .finally(() => this.submit.ready());
    }

    private validate(): boolean {
        let hasErrors = this.description.validate();
        hasErrors = this.email.validate() || hasErrors;

        return !hasErrors;
    }
}

new JobQuestion();
