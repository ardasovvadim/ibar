import {Component, Inject, OnInit} from '@angular/core';
import {MasterAccountGridVm} from '../../../master-account/master-account-grid.vm';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {FtpCredentialService} from '../ftp-credential.service';
import {FtpCredentialModel} from '../ftp-credential.model';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {Utils} from '../../../../../core/utils/utils';
import {LoadingService} from '../../../../../core/services/loading.service';

@Component({
  selector: 'app-ftp-credential-add-dialog',
  templateUrl: './ftp-credential-add-dialog.component.html',
  styleUrls: ['./ftp-credential-add-dialog.component.sass']
})
export class FtpCredentialAddDialogComponent implements OnInit {

  ftpCred: FtpCredentialModel = new FtpCredentialModel();
  form: FormGroup;
  masterAccounts: { account: MasterAccountGridVm, value: boolean }[] = [];

  constructor(private dialogRef: MatDialogRef<FtpCredentialAddDialogComponent>,
              private builder: FormBuilder,
              private loading: LoadingService,
              @Inject(MAT_DIALOG_DATA)
              private ftpCredService: FtpCredentialService) {
  }

  get controls() {
    return this.form.controls;
  }

  ngOnInit() {
    this.ftpCredService.getMasterAccounts()
      .pipe(catchError(err => of([])))
      .subscribe(data => {
        if (data != null) {
          data.forEach((acc, index) => {
            this.masterAccounts.push({account: acc, value: false});
          });
        }
      });

    this.initForm();
  }

  closeDialog(masterAccount: MasterAccountGridVm) {
    this.dialogRef.close(masterAccount);
  }

  private initForm() {
    // TODO: validation
    this.form = this.builder.group({
      ftpName: [this.ftpCred.ftpName, Validators.required],
      userName: [this.ftpCred.userName, Validators.required],
      userPassword: [this.ftpCred.userPassword, Validators.required],
      ftpUrl: [this.ftpCred.url, Validators.required]
    });
  }

  submitForm() {
    if (this.form.invalid) {
      return;
    }

    this.ftpCred.ftpName = (this.form.get('ftpName').value as string).trim();
    this.ftpCred.userName = (this.form.get('userName').value as string).trim();
    this.ftpCred.userPassword = (this.form.get('userPassword').value as string).trim();
    this.ftpCred.url = (this.form.get('ftpUrl').value as string).trim();
    this.ftpCred.masterAccounts = this.masterAccounts.filter(acc => acc.value).map(acc => acc.account);
    this.loading.setLoading(true);
    this.ftpCredService.addFtpCredential(this.ftpCred)
      .pipe(
        catchError(err => of(null)),
        finalize(() => this.loading.setLoading(false)))
      .subscribe(data => {
        if (data != null) {
          this.closeDialog(data);
        }
      });
  }

  onChangeMasterAccount(value: boolean, account: { account: MasterAccountGridVm; value: boolean }) {
    account.value = value;
  }

  resolveMasterAccountName(account: MasterAccountGridVm): string {
    return Utils.resolveMasterAccountName(account);
  }

}
