import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import {MasterAccountGridVm} from '../master-account-grid.vm';
import {MasterAccountService} from '../master-account.service';
import {LoadingService} from '../../../../core/services/loading.service';
import {catchError, finalize} from 'rxjs/operators';
import {FormHelper} from '../../../../core/utils/form-helper';
import {of} from 'rxjs';
import {Patterns} from '../../../../core/utils/patterns';

@Component({
  selector: 'app-master-account-add-dialog',
  templateUrl: './master-account-add-dialog.component.html',
  styleUrls: ['./master-account-add-dialog.component.sass']
})
export class MasterAccountAddDialogComponent implements OnInit {

  isSubmitted = false;
  isSent = false;
  masterAccount: MasterAccountGridVm = new MasterAccountGridVm();
  form: FormGroup;
  modelState: any = null;
  @ViewChild('formElRef', {static: false}) formElRef: ElementRef;

  constructor(private dialogRef: MatDialogRef<MasterAccountAddDialogComponent>,
              private formBuilder: FormBuilder,
              private masterAccountService: MasterAccountService,
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
      accountName: ['', [Validators.required, Validators.pattern(Patterns.ACCOUNT_NAME_PATTERN)]],
      accountAlias: ['', Validators.required]
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
    this.masterAccountService.addNewMasterAccount(this.masterAccount)
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
