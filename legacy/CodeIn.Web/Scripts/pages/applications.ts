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
import { ActionFile } from '@/components/action-file';

class JobApplications {

    // Fields
    public name: ActionInput;
    public email: ActionInput;
    public cover: ActionArea;
    public resume: ActionFile;

    // Submit button
    public submit: Button;

    // Dialog
    private failed: Dialog;

    public get jobId(): string | undefined { return document.querySelector("article")?.dataset.jobId }

    constructor() {

        // Fields
        this.cover = new ActionArea({
            el: document.querySelector(`[data-input-id="cover"]`) as HTMLElement
        });

        this.email = new ActionInput({
            el: document.querySelector(`[data-input-id="email"]`) as HTMLElement
        });

        this.name = new ActionInput({
            el: document.querySelector(`[data-input-id="full-name"]`) as HTMLElement
        });

        this.resume = new ActionFile({
            el: document.querySelector(`[data-input-id="resume"]`) as HTMLElement
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
            cover: this.cover.value,
            name: this.name.value,
            email: this.email.value,
            resume: this.resume.value
        };

        api
        .post(ApiRoutes.Candidates.Submissions, data)
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
            if (problems.contains("MaximumLengthValidator", "Cover")) this.cover.setError("TooLong");
            if (problems.contains("MaximumLengthValidator", "Name")) this.name.setError("TooLong");
            if (problems.containsTitle("QuestionSubmission.Job.Id")) this.failed.open();
            if (problems.containsTitle("CandidateSubmission.Resume.NotPdf")) this.resume.setError("Required");
        })
        .finally(() => this.submit.ready());
    }

    private validate(): boolean {
        let hasErrors = this.cover.validate();
        hasErrors = this.email.validate() || hasErrors;

        return !hasErrors;
    }
}

new JobApplications();
