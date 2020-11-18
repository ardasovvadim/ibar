import {Component, OnInit} from '@angular/core';
import {FtpCredentialService} from './ftp-credential.service';
import {LoadingService} from '../../../../core/services/loading.service';
import {FtpCredentialModel} from './ftp-credential.model';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {ConfirmService} from '../../../../core/services/confirm.service';
import { MatDialog } from '@angular/material/dialog';
import {FtpCredentialAddDialogComponent} from './ftp-credential-add-dialog/ftp-credential-add-dialog.component';

@Component({
  selector: 'app-ftp-credential',
  templateUrl: './ftp-credential.component.html',
  styleUrls: ['./ftp-credential.component.sass']
})
export class FtpCredentialComponent implements OnInit {

  ftpCredentials: FtpCredentialModel[] = [];

  constructor(private ftpCredService: FtpCredentialService,
              private loading: LoadingService,
              private confirm: ConfirmService,
              private dialog: MatDialog) {
  }

  ngOnInit() {
    this.refreshData();
  }

  onDeleted(ftpCred: FtpCredentialModel) {
    this.refreshData();
  }

  addFtpCredential() {
    this.dialog.open(FtpCredentialAddDialogComponent, {data: this.ftpCredService, maxWidth: 500}).afterClosed().subscribe(() => this.refreshData());
  }

  private refreshData() {
    this.loading.setLoading(true);
    this.ftpCredService.getAllFtpCredentials().pipe(
      catchError(err => {
        return of([]);
      }),
      finalize(() => this.loading.setLoading(false))
    ).subscribe(data => {
      this.ftpCredentials = data;
    });
  }
}
