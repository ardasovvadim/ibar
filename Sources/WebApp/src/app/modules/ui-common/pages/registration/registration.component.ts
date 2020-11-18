import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {Patterns} from '../../../core/helpers/patterns';
import {RegistrationService} from '../../../core/services/registration.service';
import {UserModel} from '../../../core/models/user.model';
import {StorageService} from '../../../core/services/storage.service';
import {FormHelper} from '../../../core/utils/form-helper';
import {catchError, finalize} from 'rxjs/operators';
import {LoadingService} from '../../../core/services/loading.service';
import {of} from 'rxjs';
import {ModelStateHelper} from '../../../core/utils/model-state-helper';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.sass']
})
export class RegistrationComponent implements OnInit {

  user: UserModel;
  form: FormGroup;
  hidePassword = true;
  isSubmitted = false;
  isSent = false;
  modelState: any = null;
  hideConfirmPassword = true;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;
  errorMessage = '';

  get controls() {
    return this.form.controls;
  }

  constructor(private router: Router,
              private regService: RegistrationService,
              private formBuilder: FormBuilder,
              private storage: StorageService,
              private loading: LoadingService) {
  }

  ngOnInit() {
    this.user = JSON.parse(this.storage.getFromRepository('registrationUser'));
    this.initForm();
  }

  private initForm() {
    this.form = this.formBuilder.group({
      password: ['', [Validators.required, Validators.pattern(Patterns.PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]]
    }, {validators: this.isMatchPasswords});
  }

  finishRegistration() {
    this.isSubmitted = true;

    if (this.form.invalid || this.isSent) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formElRef);
      }
      return;
    }

    this.modelState = null;
    this.isSent = true;

    this.user.password = this.form.get('password').value;

    this.loading.setLoading(true);
    this.regService.finishRegistration(this.user)
      .pipe(
        finalize(() => {
          this.loading.setLoading(false);
          this.isSent = false;
        }),
        catchError(err => {
          this.modelState = err.error.modelState;
          if (ModelStateHelper.contains('codePhone', this.modelState)) {
            this.errorMessage = ModelStateHelper.getFirstOrEmpty('codePhone', this.modelState);
          } else if (ModelStateHelper.contains('password', this.modelState)) {
            this.errorMessage = ModelStateHelper.getFirstOrEmpty('password', this.modelState);
          }
          FormHelper.processModelState(this.form, this.modelState, this.formElRef);
          return of(null);
        })
      )
      .subscribe(data => {
        if (data != null) {
          this.regService.Clear();
          if (data === '/') {
            this.router.navigate(['/']);
          } else {
            location.href = data;
          }
        }
      });
  }

  isMatchPasswords(form: FormGroup) {
    const control = form.get('password');
    const matchingControl = form.get('confirmPassword');

    if (matchingControl.errors && !matchingControl.errors.invalidConfirmPassword) {
      return;
    }

    if (control.value !== matchingControl.value) {
      matchingControl.setErrors({invalidConfirmPassword: true});
    } else {
      matchingControl.setErrors(null);
    }
  }

}
