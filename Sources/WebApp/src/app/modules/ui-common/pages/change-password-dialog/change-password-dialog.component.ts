import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ChangePasswordModel} from './change-password.model';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {MatDialogRef} from '@angular/material/dialog';
import {LoadingService} from '../../../core/services/loading.service';
import {FormHelper} from '../../../core/utils/form-helper';
import {Patterns} from '../../../core/helpers/patterns';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {finalize} from 'rxjs/operators';

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.sass']
})
export class ChangePasswordDialogComponent implements OnInit {

  isSubmitted = false;
  isSent = false;
  form: FormGroup;
  modelState: any = null;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;
  passwordModel: ChangePasswordModel = new ChangePasswordModel();
  // ---
  hideOldPassword = true;
  hideNewPassword = true;
  hideConfirmNewPassword = true;

  constructor(private dialogRef: MatDialogRef<ChangePasswordDialogComponent>,
              private formBuilder: FormBuilder,
              private authService: AuthenticationService,
              private loading: LoadingService) {
  }

  get controls() {
    return this.form.controls;
  }

  ngOnInit() {
    this.initForm();
  }

  closeDialog(result: number) {
    this.dialogRef.close(result);
  }

  private initForm() {
    this.form = this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.pattern(Patterns.PASSWORD_PATTERN)]],
      confirmNewPassword: ['', Validators.required],
    });
  }

  submitForm() {
    this.isSubmitted = true;

    this.checkPasswords();

    if (this.isSent || this.form.invalid) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formElRef);
      }
      return;
    }

    this.modelState = null;
    this.isSent = true;

    this.passwordModel.id = this.authService.currentUserValue.id;
    this.passwordModel.oldPassword = this.form.get('oldPassword').value.trim();
    this.passwordModel.newPassword = this.form.get('newPassword').value.trim();

    this.loading.setLoading(true);
    this.authService.changePassword(this.passwordModel)
      .pipe(
        finalize(() => {
          this.loading.setLoading(false);
          this.isSent = false;
        })
      ).subscribe(
      () => this.closeDialog(1),
      err => {
        this.modelState = err.error.modelState;
        FormHelper.processModelState(this.form, this.modelState, this.formElRef);
      }
    );
  }

  private checkPasswords() {
    const pas1 = this.form.get('newPassword').value.trim();
    const pas2 = this.form.get('confirmNewPassword').value.trim();

    if (!pas1 && !pas2 && pas1 !== pas2) {
      this.form.get('confirmNewPassword').setErrors({passwordMismatch: true});
    }
  }
}
