import {Component, OnInit, ViewChild} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import {MasterAccountService} from './master-account.service';
import {MasterAccountGridVm} from './master-account-grid.vm';
import {DateConvector} from '../../../core/utils/date-convector';
import {MasterAccountAddDialogComponent} from './master-account-add-dialog/master-account-add-dialog.component';
import {LoadingService} from '../../../core/services/loading.service';
import {finalize} from 'rxjs/operators';
import {MasterAccountEditDialogComponent} from './master-account-edit-dialog/master-account-edit-dialog.component';
import {ConfirmService} from '../../../core/services/confirm.service';

@Component({
  selector: 'app-master-account',
  templateUrl: './master-account.component.html',
  styleUrls: ['./master-account.component.sass']
})
export class MasterAccountComponent implements OnInit {

  displayedColumns: string[] = ['position', 'accountName', 'accountAlias', 'createdDate', 'createdBy', 'modifiedDate', 'updatedBy', 'amountTradeAccounts', 'tools'];
  masterAccounts: MasterAccountGridVm[];
  @ViewChild(MatTable, {static: true}) table: MatTable<any>;

  constructor(private masterAccountService: MasterAccountService,
              private dialog: MatDialog,
              private loading: LoadingService,
              private confirm: ConfirmService) {
  }

  ngOnInit() {
    this.loading.setLoading(true);
    this.masterAccountService.getAllMasterAccounts().pipe(
      finalize(() => this.loading.setLoading(false)
      )).subscribe(
      data => {
        if (data) {
          this.masterAccounts = data;
        } else {
          this.masterAccounts = [];
        }
      });
  }

  addMasterAccount() {
    this.dialog.open(MasterAccountAddDialogComponent).afterClosed().subscribe(data => this.refreshData());
  }

  deleteMasterAccount(masterAccount: MasterAccountGridVm) {
    this.confirm.ShowConfirmDialog().subscribe(answer => {
      if (answer) {
        this.masterAccountService.deleteMasterAccount(masterAccount).subscribe(data => this.refreshData());
      }
    });
  }


  editMasterAccount(masterAccount: MasterAccountGridVm) {
    this.dialog.open(MasterAccountEditDialogComponent, {data: {...masterAccount}}).afterClosed().subscribe(data => this.refreshData());
  }

  refreshData() {
    this.loading.setLoading(true);
    this.masterAccountService.getAllMasterAccounts(true).pipe(
      finalize(() => this.loading.setLoading(false)
      )).subscribe(
      data => {
        if (data) {
          this.masterAccounts = data;
        } else {
          this.masterAccounts = [];
        }
      });
  }
}
