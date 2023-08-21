import { FormControl } from "@angular/forms";


export function lowerCaseValidator(control: FormControl): {
    [key: string]: boolean;
} | null {
    const value: string = control.value;

    if (value && value !== value.toLowerCase()) {
        return { 'lowerCaseValidator': true };
    } else {
        return null;
    }
}
