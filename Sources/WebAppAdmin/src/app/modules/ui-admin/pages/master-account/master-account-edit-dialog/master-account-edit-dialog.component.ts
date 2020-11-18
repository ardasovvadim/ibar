import {Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {MasterAccountGridVm} from '../master-account-grid.vm';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {MasterAccountService} from '../master-account.service';
import {LoadingService} from '../../../../core/services/loading.service';
import {Patterns} from '../../../../core/utils/patterns';
import {catchError, finalize} from 'rxjs/operators';
import {FormHelper} from '../../../../core/utils/form-helper';
import {of} from 'rxjs';

@Component({
  selector: 'app-master-account-edit-dialog',
  templateUrl: './master-account-edit-dialog.component.html',
  styleUrls: ['./master-account-edit-dialog.component.sass']
})
export class MasterAccountEditDialogComponent implements OnInit {

  isSubmitted = false;
  isSent = false;
  form: FormGroup;
  modelState: any = null;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;

  constructor(private dialogRef: MatDialogRef<MasterAccountEditDialogComponent>,
              private formBuilder: FormBuilder,
              private masterAccountService: MasterAccountService,
              @Inject(MAT_DIALOG_DATA) public masterAccount: MasterAccountGridVm,
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
      accountName: [this.masterAccount.accountName, [Validators.required, Validators.pattern(Patterns.ACCOUNT_NAME_PATTERN)]],
      accountAlias: [this.masterAccount.accountAlias, Validators.required]
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

    this.masterAccount.accountName = this.controls.accountName.value.trim();
    this.masterAccount.accountAlias = this.controls.accountAlias.value.trim();
    this.loading.setLoading(true);
    this.masterAccountService.editMasterAccount(this.masterAccount)
      .pipe(
        finalize(() => {
          this.loading.setLoading(false);
          this.isSent = false;
        }),
        catchError(err => {
          this.modelState = err.error.modelState;
          FormHelper.processModelState(this.form, this.modelState, this.formElRef);
          return of(-1);
        })
      )
      .subscribe(data => {
        if (data !== -1) {
          this.closeDialog(data);
        }
      });
  }

}
