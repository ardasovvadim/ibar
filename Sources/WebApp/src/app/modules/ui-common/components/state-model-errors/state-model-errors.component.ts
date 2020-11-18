import {Component, Input, OnInit} from '@angular/core';
import {AbstractControl, FormGroup} from '@angular/forms';

@Component({
  selector: 'app-state-model-errors',
  templateUrl: './state-model-errors.component.html',
  styleUrls: ['./state-model-errors.component.sass']
})
export class StateModelErrorsComponent implements OnInit {


  private _modelState: any = null;
  modelErrors: string[] = [];
  control: AbstractControl = null;


  @Input() form: FormGroup;
  @Input() controlName: string;
  @Input() customErrors: { error: string, message: string }[] = [];
  @Input() submitted = true;

  @Input() set modelState(val: any) {
    this._modelState = val;
    this.processModelState();
  }


  constructor() {
  }

  ngOnInit() {
    this.control = this.form.get(this.controlName);
  }

  private processModelState() {
    if (this._modelState != null) {
      const key = Object.keys(this._modelState)
        .find(k => k.toLocaleLowerCase().includes(this.controlName.toLocaleLowerCase()));
      if (typeof key !== 'undefined') {
        this.modelErrors = this._modelState[key];
        // this.control.markAsTouched();
        return;
      }
    }

    this.modelErrors = [];
  }

  resolveCustomErrorMessage(): string {
    let message = '';
    if (this.control
      && this.control.invalid
      && this.submitted
      && this.control.pristine) {
      for (const err of this.customErrors) {
        if (this.form.get(this.controlName).errors[err.error]) {
          message = err.message;
          break;
        }
      }
    }
    return message;
  }

  resolveModelStateError(): string {
    let message = '';
    if (this.control.invalid
      && this.control.hasError('modelState')) {
      message = this.modelErrors[0];
    }
    return message;
  }
}
