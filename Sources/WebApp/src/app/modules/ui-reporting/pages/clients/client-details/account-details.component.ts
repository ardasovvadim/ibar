import {Component, OnInit} from '@angular/core';
import {ClientsService} from '../clients.service';
import {ActivatedRoute} from '@angular/router';
import {AccountDetailsModel} from './models/account-details.model';
import {FormBuilder, FormGroup} from '@angular/forms';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {TradingPermissionModel} from './models/trading-permission.model';
import {MatDialog} from '@angular/material/dialog';
import {TradeAccountNoteModel} from './note/trade-account-note.model';
import {NoteAddDialogComponent} from './note-add-dialog/note-add-dialog.component';
import {TradeRank} from './models/trade-rank.enum';
import {MatCheckboxChange} from '@angular/material/checkbox';
import {LoadingService} from '../../../../core/services/loading.service';
import {DateConvector} from '../../../../core/utils/date-convector';

@Component({
  selector: 'app-client-account',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.sass']
})
export class AccountDetailsComponent implements OnInit {

  private accountNameParam: string;
  accountInfo: AccountDetailsModel = new AccountDetailsModel();
  form: FormGroup;
  tradingPermissions: { perm: TradingPermissionModel, value: boolean }[] = [];
  tradeRankEnum = TradeRank;

  constructor(private route: ActivatedRoute,
              private clientsService: ClientsService,
              private fBuilder: FormBuilder,
              private loading: LoadingService,
              private dialog: MatDialog) {
  }

  ngOnInit() {
    this.route.parent.params.subscribe(params => {
      this.accountNameParam = params['id'];
      this.refreshData();
    });

    this.initForms();
  }

  private initForms() {
    this.form = this.fBuilder.group({
      name: [this.accountInfo.name],
      city: [this.accountInfo.cityResidentialAddress],
      postCode: [this.accountInfo.postalCode],
      country: [this.accountInfo.countryResidentialAddress],
      street: [this.accountInfo.streetResidentialAddress],
      email: [this.accountInfo.primaryEmail],
      accountName: [this.accountInfo.accountName],
      accountType: [this.accountInfo.customerType],
      dateOpened: [this.accountInfo.dateOpened],
      currency: [this.accountInfo.currency],
      customerType: [this.accountInfo.customerType],
      dateFunded: [this.accountInfo.dateFunded],
      cash: [''],
      masterAccount: [this.accountInfo.masterAccount],
      dateClosed: [this.accountInfo.dateClosed]
    });
  }

  dateToString(date: Date): string {
    return DateConvector.DateToString(date);
  }

  checkEmpty(str: string): string {
    return str ? str : '';
  }

  private loadAccountInfo() {
    this.loading.setLoading(true);
    this.clientsService.getAccountInfo(this.accountNameParam)
      .pipe(
        catchError(err => {
          return of(new AccountDetailsModel());
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data != null) {
          this.accountInfo = data;
          this.clientsService.setAccountHeader({id: data.accountName, name: data.name});
          this.accountInfo.tradingPermissions.forEach(perm => {
            this.tradingPermissions.find(findPerm => perm.id === findPerm.perm.id).value = true;
          });
        } else {
          this.accountInfo = new AccountDetailsModel();
        }
      });
  }

  addNote() {
    this.dialog.open(NoteAddDialogComponent, {data: this.accountInfo.id, width: '600px'})
      .afterClosed()
      .subscribe(data => {
        if (data) {
          this.refreshData();
        }
      });
  }

  private refreshData() {
    this.tradingPermissions = [];

    this.loading.setLoading(true);
    this.clientsService.getTradingPermissions()
      .pipe(
        catchError(err => of([])),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        data.forEach(perm => {
          this.tradingPermissions.push({perm, value: false});
        });
        this.loadAccountInfo();
      });
  }

  deleteNote($event: TradeAccountNoteModel) {
    this.loading.setLoading(true);
    this.clientsService.deleteTradeAccountNote($event.id)
      .pipe(
        catchError((err) => {
          return of(0);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data !== 0) {
          this.accountInfo.tradeAccountNotes = this.accountInfo.tradeAccountNotes.filter(note => note.id !== data);
        }
      });
  }

  changeRank(rank: TradeRank) {
    this.clientsService.changeTradeAccountRank({id: this.accountInfo.id, tradeRank: rank})
      .pipe(
        catchError(err => {
          return of(-1);
        })
      )
      .subscribe(data => {
        if (data !== -1) {
          this.refreshData();
        }
      });
  }

  discardChanges(checkbox: MatCheckboxChange) {
    checkbox.source.checked = !checkbox.checked;
  }
}
