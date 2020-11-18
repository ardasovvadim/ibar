import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FtpCredentialModel} from '../ftp-credential.model';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {MasterAccountGridVm} from '../../../master-account/master-account-grid.vm';
import {FtpCredentialService} from '../ftp-credential.service';
import {auditTime, catchError, finalize, map} from 'rxjs/operators';
import {of} from 'rxjs';
import {ConfirmService} from '../../../../../core/services/confirm.service';
import {LoadingService} from '../../../../../core/services/loading.service';

@Component({
  selector: 'app-ftp-credential-panel',
  templateUrl: './ftp-credential-panel.component.html',
  styleUrls: ['./ftp-credential-panel.component.sass']
})
export class FtpCredentialPanelComponent implements OnInit {

  private autoSaveTime = 2000;
  @Input() ftpCred: FtpCredentialModel;
  @Output() deleted = new EventEmitter<FtpCredentialModel>();
  form: FormGroup;
  masterAccounts: { account: MasterAccountGridVm, value: boolean }[] = [];

  constructor(private builder: FormBuilder,
              private ftpCredService: FtpCredentialService,
              private confirmService: ConfirmService,
              private loading: LoadingService) {
  }

  ngOnInit() {
    this.ftpCredService.getMasterAccounts().pipe(
      catchError(err => of([]))
    ).subscribe(data => {
      if (data != null) {
        data.forEach((acc, index) => {
          this.masterAccounts.push({account: acc, value: false});
        });
        this.ftpCred.masterAccounts.forEach(acc => {
          this.masterAccounts.find(findAcc => findAcc.account.id === acc.id).value = true;
        });
      }
    });

    this.form = this.builder.group({
      ftpName: [this.ftpCred.ftpName, Validators.required],
      userName: [this.ftpCred.userName, Validators.required],
      userPassword: [this.ftpCred.userPassword, Validators.required],
      ftpUrl: [this.ftpCred.url, Validators.required]
    });

    this.form.get('ftpName').valueChanges.subscribe((data: string) => {
      this.ftpCred.ftpName = data.trim();
    });

    this.form.valueChanges.pipe(auditTime(this.autoSaveTime)).subscribe(data => {
      if (this.form.invalid) {
        return;
      }

      this.ftpCred.ftpName = (this.form.get('ftpName').value as string).trim();
      this.ftpCred.userName = (this.form.get('userName').value as string).trim();
      this.ftpCred.userPassword = (this.form.get('userPassword').value as string).trim();
      this.ftpCred.url = (this.form.get('ftpUrl').value as string).trim();
      this.ftpCred.masterAccounts = this.masterAccounts.filter(acc => acc.value).map(acc => acc.account);

      this.ftpCredService.updateFtpCredential(this.ftpCred)
        .pipe(catchError(err => {
          return of(null);
        }))
        .subscribe((data) => {
        });
    });
  }

  resolveMasterAccountName(masterAccount: MasterAccountGridVm): string {
    return (typeof masterAccount.accountAlias !== 'undefined' && masterAccount.accountAlias)
      ? masterAccount.accountAlias : masterAccount.accountName;
  }

  onChangeMasterAccount(value: boolean, masterAccountModel: { account: MasterAccountGridVm; value: boolean }) {
    masterAccountModel.value = value;
    this.ftpCred.masterAccounts = this.masterAccounts.filter(acc => acc.value).map(acc => acc.account);
    this.form.updateValueAndValidity({onlySelf: false, emitEvent: true});
  }

  delete() {
    this.confirmService.ShowConfirmDialog().subscribe(answer => {
      if (answer) {
        this.loading.setLoading(true);
        this.ftpCredService.deleteFtpCredential(this.ftpCred)
          .pipe(
            finalize(() => this.loading.setLoading(false)),
            map(() => true),
            catchError(() => of(false))
          )
          .subscribe((value: boolean) => {
            if (value) {
              this.deleted.emit(this.ftpCred);
            }
          });
      }
    });
  }
}
