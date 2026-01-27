import { StringUtilities } from "./string-utilities";

export class Highlighter {

    public static Elements(search: string, ...elements: HTMLElement[]) {

        const s = StringUtilities.ToUnaccent(search.trim().toLowerCase());

        elements.forEach(e => {

            let output = "";
            let found = false;

            const t = StringUtilities.ToUnaccent(e.innerText.toLowerCase());

            if (s && t) {

                let last = 0;
                let index = t.indexOf(s, last);

                while (index > -1) {

                    found = true;

                    output += e.innerText.substring(last, index) + "<mark>" + e.innerText.substring(index, index + s.length) + "</mark>";

                    last = index + s.length;
                    index = t.indexOf(s, last);
                }

                output += e.innerText.substring(last);
            }
            
            e.innerHTML = found ? output : e.innerText;

        });
        
    }
}