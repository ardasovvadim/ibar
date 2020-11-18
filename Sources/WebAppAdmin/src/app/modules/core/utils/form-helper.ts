import {FormGroup} from '@angular/forms';
import {ElementRef} from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';


export class FormHelper {
  static focusFirstControlOnError(form: FormGroup, formElRef: ElementRef): void {
    for (const key of Object.keys(form.controls)) {
      if (form.controls[key].invalid) {
        const invalidControl = formElRef.nativeElement.querySelector(`[formcontrolname="${key}"`);
        invalidControl.focus();
        this.resetStates(form);
        return;
      }
    }
  }

  static focusFirstControl(form: FormGroup, formElRef: ElementRef): void {
    const keys = Object.keys(form.controls);
    if (keys.length > 0) {
      const key = keys[0];
      const control = formElRef.nativeElement.querySelector(`[formcontrolname="${key}"`);
      control.focus();
    }
  }

  static processModelState(form: FormGroup, modelState: any, formElRef: ElementRef = null): void {
    if (modelState != null) {
      const controls = Object.keys(form.controls);
      Object.keys(modelState).forEach(key => {
        const ctrlKey = controls.find(ctrl => key.toLocaleLowerCase().includes(ctrl.toLocaleLowerCase()));
        if (typeof ctrlKey !== 'undefined') {
          form.controls[ctrlKey].setErrors({modelState: true}, {emitEvent: true});
        }
      });

      if (formElRef != null) {
        this.focusFirstControlOnError(form, formElRef);
      }

      this.resetStates(form);
    }
  }

  static resetStates(form: FormGroup) {
    Object.keys(form.controls).forEach(key => {
      form.get(key).markAsUntouched();
      form.get(key).markAsPristine();
    });
  }

}
