import {Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {UserModel} from '../user.model';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {UserService} from '../user.service';
import {Patterns} from '../../../../core/utils/patterns';
import {LoadingService} from '../../../../core/services/loading.service';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {FormHelper} from '../../../../core/utils/form-helper';

@Component({
  selector: 'app-user-edit-dialog',
  templateUrl: './user-edit-dialog.component.html',
  styleUrls: ['./user-edit-dialog.component.sass']
})
export class UserEditDialogComponent implements OnInit {

  isSubmitted = false;
  isSent = false;
  form: FormGroup;
  modelState: any = null;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;

  constructor(private dialogRef: MatDialogRef<UserEditDialogComponent>,
              private formBuilder: FormBuilder,
              private userService: UserService,
              @Inject(MAT_DIALOG_DATA) public user: UserModel,
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
      firstName: [this.user.firstName, Validators.required],
      lastName: [this.user.lastName, Validators.required],
      email: [this.user.email, [Validators.required, Validators.pattern(Patterns.EMAIL_PATTERN), Patterns.checkDomain]],
      phone: [this.user.phone, [Validators.required, Validators.pattern(Patterns.PHONE_PATTERN)]]
    });
  }

  submitForm() {
    this.isSubmitted = true;

    if (this.isSent || this.form.invalid) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formElRef)
      }
      return;
    }

    this.modelState = null;
    this.isSent = true;

    this.user.firstName = this.controls.firstName.value.trim();
    this.user.lastName = this.controls.lastName.value.trim();
    this.user.email = this.controls.email.value.trim();
    this.user.phone = this.controls.phone.value.trim();
    this.loading.setLoading(true);
    this.userService.updateUser(this.user).pipe(
      finalize(() => {
        this.loading.setLoading(false);
        this.isSent = false;
      }),
      catchError(err => {
        this.modelState = err.error.modelState;
        FormHelper.processModelState(this.form, this.modelState, this.formElRef);
        return of(-1);
      })
    ).subscribe(data => {
        if (data !== -1) {
          this.closeDialog(data);
        }
      }
    );
  }

}
