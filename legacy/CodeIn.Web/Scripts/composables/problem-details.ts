import { AxiosError } from "axios";

export class Problem
{
    public code: string = "";
    public description: string = "";
    public field: string = "";
    public title: string = "";
    public type: string = "";

    constructor(p: any) {
        this.code = Problem.extract(p.code);
        this.description = Problem.extract(p.description);
        this.field = Problem.extract(p.field);
        this.title = Problem.extract(p.title);
        this.type = Problem.extract(p.type);
    }

    private static extract(x: any): string { return typeof x === 'string' ? x : "" }
}

export class ProblemDetails
{
    public problems: Problem[] = [];

    constructor(e?: AxiosError) {
        const data = e?.response?.data as any;
        if (!data) return
        else if (data.errors && Array.isArray(data.errors))
        {
            this.problems = data.errors.map((x: any) => new Problem(x));
        }
        else
        {
            this.problems = [new Problem(data)];
        }
    }

    public contains(code: string, field: string): boolean
    {
        return code.length > 0 
            && field.length > 0 
            && this.problems.some(p => p.code === code && p.field === field );
    }

    public containsTitle(title: string): boolean
    {
        return title.length > 0 
            && this.problems.some(p => p.title === title );
    }

}