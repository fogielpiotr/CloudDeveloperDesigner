import { AbstractControl, FormControl, Validators } from '@angular/forms';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-form-control-error',
  templateUrl: './form-control-error.component.html',
  styleUrls: ['./form-control-error.component.scss']
})
export class FormControlErrorComponent implements OnInit {

  @Input()
  control: any = null;
  constructor() { }

  ngOnInit(): void {
  }

  getError(control: AbstractControl): string {
    if (control.errors[Validators.required.name]) {
      return "Value is required.";
    }
    if (control.errors[Validators.minLength.name.toLowerCase()]) {
      return `Value should contain at least ${control.errors[Validators.minLength.name.toLowerCase()].requiredLength} characters.`;
    }
    if (control.errors[Validators.maxLength.name.toLowerCase()]) {
      return `Value shouldn't contain more than ${control.errors[Validators.maxLength.name.toLowerCase()].requiredLength} characters.`;
    }
    if (control.errors['noWhitespaceValidator']) {
      return `Value shouldn't contain space.`
    }
    if (control.errors['lowerCaseValidator']) {
      return `Value should be written in lowercase.`
    }
    return 'Value is not valid';
  }

}
