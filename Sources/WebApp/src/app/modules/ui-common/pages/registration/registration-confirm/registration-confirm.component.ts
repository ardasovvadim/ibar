import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {RegistrationService} from '../../../../core/services/registration.service';
import {Router} from '@angular/router';
import {Patterns} from '../../../../core/helpers/patterns';
import {catchError, finalize, first} from 'rxjs/operators';
import {FormHelper} from '../../../../core/utils/form-helper';
import {of, throwError} from 'rxjs';
import {LoadingService} from '../../../../core/services/loading.service';
import {ModelStateHelper} from '../../../../core/utils/model-state-helper';
import {ExceptionHeaderService} from '../../../../core/services/exception-header.service';

@Component({
  selector: 'app-registration-confirm',
  templateUrl: './registration-confirm.component.html',
  styleUrls: ['./registration-confirm.component.sass']
})
export class RegistrationConfirmComponent implements OnInit {

  form: FormGroup;
  isSubmitted = false;
  modelState: any = null;
  private isSent = false;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;
  errorMessage = '';

  constructor(private regService: RegistrationService,
              private router: Router,
              private formBuilder: FormBuilder,
              private loading: LoadingService,
              private exceptionHeaderService: ExceptionHeaderService) {
  }

  ngOnInit() {
    this.initForm();
    this.sendRegistrationCode();
  }

  private sendRegistrationCode() {
    this.loading.setLoading(true);
    this.regService.sendRegistrationCode()
      .pipe(
        finalize(() => this.loading.setLoading(false)),
        catchError(err => {
          const modelState = err.error.modelState;
          if (ModelStateHelper.contains('phoneCode', modelState)) {
            this.errorMessage = ModelStateHelper.getFirstOrEmpty('phoneCode', modelState);
            this.form.valueChanges.pipe(first()).subscribe(() => this.errorMessage = '');
          }
          return throwError(err);
        }))
      .subscribe();
  }

  get controls() {
    return this.form.controls;
  }

  confirmRegistration() {
    this.isSubmitted = true;

    if (this.form.invalid || this.isSent) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formElRef);
      }
      return;
    }

    this.isSent = true;
    this.modelState = null;

    const verificationCode = this.controls.phoneCode.value as number;

    this.loading.setLoading(true);
    this.regService.confirmRegistration(verificationCode)
      .pipe(
        finalize(() => {
          this.loading.setLoading(false);
          this.isSent = false;
        }),
        catchError(err => {
          this.modelState = err.error.modelState;
          FormHelper.processModelState(this.form, this.modelState, this.formElRef);
          return of(null);
        })
      ).subscribe(data => {
      if (data != null) {
        this.router.navigate(['../registration']);
      }
    });
  }

  private initForm() {
    this.form = this.formBuilder.group({
      phoneCode: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]],
    });
  }

}
