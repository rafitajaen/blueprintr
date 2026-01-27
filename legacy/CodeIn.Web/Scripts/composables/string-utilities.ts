export class StringUtilities {
    public static ToUnaccent(s: string) {
        return s.normalize("NFD").replace(/[\u0300-\u306f]/g, "")
    }
}