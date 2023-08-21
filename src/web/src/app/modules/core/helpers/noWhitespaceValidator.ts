import { FormControl } from "@angular/forms";



export function noWhitespaceValidator(control: FormControl): {
    [key: string]:
    boolean
} | null {
    const isSpace = (control.value || '').match(/\s/g);
    return isSpace ? { 'noWhitespaceValidator': true } : null;
}