import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import {Patterns} from '../../../../core/utils/patterns';
import {UserModel} from '../user.model';
import {UserService} from '../user.service';
import {LoadingService} from '../../../../core/services/loading.service';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {FormHelper} from '../../../../core/utils/form-helper';

@Component({
  selector: 'app-user-add-dialog',
  templateUrl: './user-add-dialog.component.html',
  styleUrls: ['./user-add-dialog.component.sass']
})
export class UserAddDialogComponent implements OnInit {

  isSubmitted = false;
  isSent = false;
  user: UserModel = new UserModel();
  form: FormGroup;
  modelState: any = null;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;

  constructor(private dialogRef: MatDialogRef<UserAddDialogComponent>,
              private formBuilder: FormBuilder,
              private userService: UserService,
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
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(Patterns.EMAIL_PATTERN), Patterns.checkDomain]],
      phone: ['', [Validators.required, Validators.pattern(Patterns.PHONE_PATTERN)]]
    });
  }

  submitForm() {
    this.isSubmitted = true;

    if (this.isSent || this.form.invalid) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formElRef);
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
    this.userService.addNewUser(this.user).pipe(
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
