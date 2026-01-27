import '@/shared/header';
import '@/shared/shared';
import '@/shared/suscribe';
import '@styles/styles.scss';

import { Button } from '@/components/button';

class Jobs
{
    public showFilters: Button;
    public apply: Button;
    public applyFilters: Button;

    constructor() {

        this.showFilters = new Button({
            el: document.getElementById("show-filters") as HTMLButtonElement,
            onClick: this.onShowFilters.bind(this)
        });
        this.apply = new Button({
            el: document.getElementById("apply") as HTMLButtonElement,
            onClick: this.onApplyFilters.bind(this)
        });
        this.applyFilters = new Button({
            el: document.getElementById("apply-filters") as HTMLButtonElement,
            onClick: this.onApplyFilters.bind(this)
        });

        // document.addEventListener("change", this.onCheckboxChange.bind(this));
    }

    private onShowFilters()
    {
        document.querySelector("aside.filters")?.classList.toggle("open");
    }

    private onApplyFilters()
    {
        const kinds = this.getKinds();
        const url = new URL(window.location.href);

        kinds.forEach(kind => {
            const selected = this.getCheckedOfKind(kind);

            if (selected.length > 0) url.searchParams.set(kind, selected.join('.'));
            else url.searchParams.delete(kind);
        });

        const kind = "Salary";
        const salary = (document.getElementById("salaries") as HTMLSelectElement).value;

        if (salary && salary !== "From0k") url.searchParams.set(kind, salary);
        else url.searchParams.delete(kind);

        window.location.href = url.toString();
    }

    // private onCheckboxChange(e: Event)
    // {
    //     if (!e?.target) return;

    //     const target = e.target as HTMLInputElement;
    //     const kind = target.dataset.kind;

    //     if (!kind) return;

    //     const url = new URL(window.location.href);
    //     const selected = this.getCheckedOfKind(kind);

    //     if (selected.length > 0) {
    //         url.searchParams.set(kind, selected.join('.'));
    //     } else {
    //         url.searchParams.delete(kind);
    //     }

    //     window.location.href = url.toString();

    // }

    private getCheckedOfKind(kind: string): string[] {

        const names: string[] = [];
        const checkboxes = document.querySelectorAll(`input[type="checkbox"][data-kind="${kind}"]`);

        checkboxes.forEach((x) => {
            if (x instanceof HTMLInputElement && x.checked && x.name) {
                names.push(x.name);
            }
        });

        return names;
    }

    private getKinds(): string[] {
        const elements = document.querySelectorAll('input[type="checkbox"][data-kind]');
        const kinds = new Set<string>();

        elements.forEach((el) => {
            const kind = el.getAttribute('data-kind');
            if (kind) kinds.add(kind);
        });

        return Array.from(kinds);
        }

}

new Jobs();
