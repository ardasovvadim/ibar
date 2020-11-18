import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AuthenticationService} from '../../../../core/services/authentication.service';
import {Patterns} from '../../../../core/utils/patterns';
import {LoadingService} from '../../../../core/services/loading.service';
import {catchError, finalize} from 'rxjs/operators';
import {RouterService} from '../../../../core/services/router.service';
import {of} from 'rxjs';
import {FormHelper} from '../../../../core/utils/form-helper';

@Component({
  selector: 'app-login-confirm',
  templateUrl: './login-confirm.component.html',
  styleUrls: ['./login-confirm.component.sass']
})
export class LoginConfirmComponent implements OnInit {

  form: FormGroup;
  isSent = false;
  isSubmitted = false;
  modelState: any = null;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;

  constructor(private authService: AuthenticationService,
              private router: RouterService,
              private formBuilder: FormBuilder,
              private loading: LoadingService) {
  }

  ngOnInit() {
    this.initForm();
  }

  get controls() {
    return this.form.controls;
  }

  confirmLogin() {
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
    this.authService.confirmLogin(verificationCode)
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
      )
      .subscribe(data => {
        if (data != null) {
          this.router.navigate();
        }
      });
  }

  private initForm() {
    this.form = this.formBuilder.group({
      phoneCode: ['', [Validators.required, Validators.pattern(Patterns.PHONE_CODE_PATTERN)]],
    });
  }

}
