import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {Patterns} from '../../../core/utils/patterns';
import {catchError, finalize, first} from 'rxjs/operators';
import {LoadingService} from '../../../core/services/loading.service';
import {of} from 'rxjs';
import {FormHelper} from '../../../core/utils/form-helper';
import {ModelStateHelper} from '../../../core/utils/model-state-helper';
import {RouterService} from '../../../core/services/router.service';
import {UserModel} from '../../../core/models/user.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.sass']
})
export class LoginComponent implements OnInit {

  user: UserModel = new UserModel();
  form: FormGroup;
  isCodeSent = false;
  hidePassword = true;
  modelState: any;
  isSubmitted = false;
  isSent = false;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;
  errorMessage: string = null;

  get controls() {
    return this.form.controls;
  }

  constructor(private router: RouterService,
              private authService: AuthenticationService,
              private formBuilder: FormBuilder,
              private loading: LoadingService) {
  }

  ngOnInit() {
    this.initForm();
  }

  private initForm() {
    this.form = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern(Patterns.EMAIL_PATTERN), Patterns.checkDomain]],
      password: ['', Validators.required],
    });
  }

  sendCode() {
    this.isSubmitted = true;

    if (this.form.invalid || this.isCodeSent) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formElRef);
      }
      return;
    }

    this.modelState = null;
    this.isCodeSent = true;

    this.user.email = this.form.controls.email.value;
    this.user.password = this.form.controls.password.value;
    this.loading.setLoading(true);
    this.authService.login(this.user).pipe(
      finalize(() => {
        this.isCodeSent = false;
        this.loading.setLoading(false);
      }),
      catchError(err => {
        this.modelState = err.error.modelState;
        FormHelper.processModelState(this.form, this.modelState, this.formElRef);
        if (ModelStateHelper.contains('invalidLoginData', this.modelState)) {
          this.errorMessage = ModelStateHelper.getFirstOrEmpty('invalidLoginData', this.modelState);
          FormHelper.focusFirstControl(this.form, this.formElRef);
          this.form.valueChanges.pipe(first()).subscribe(() => {
            this.errorMessage = null;
          });
        } else {
          this.errorMessage = null;
        }
        return of(null);
      })
    ).subscribe(data => {
      if (data != null) {
        this.router.navigate('login/confirm');
      }
    });
  }

}
