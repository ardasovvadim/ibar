import {Component, ComponentFactoryResolver, OnInit, ViewChild, ViewContainerRef} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Period} from '../../../../core/models/period';
import {TotalClientTotalsEnum} from './client-trades-totals.enum';
import {ClientsTradesService} from './clients-trades.service';
import {MatTable} from '@angular/material/table';
import {TotalClientsTableModel} from '../../total-clients/total-clients-table.model';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {LoadingService} from '../../../../core/services/loading.service';
import {ClientsService} from '../clients.service';
import {AccountTradesDetailsModel} from './client-trade-details.model';

@Component({
  selector: 'app-client-trades',
  templateUrl: './client-trades.component.html',
  styleUrls: ['./client-trades.component.sass'],
  providers: [ClientsTradesService]
})
export class ClientTradesComponent implements OnInit {

  availableTradesDate: Date[] = [];
  tradeDataLength = 0;
  tradesPageLength = 10;
  tradesPageIndex = 0;
  currentTradesIndex = 1;
  selectedTradesStrDate = '';
  tradeStartDate: Date = null;
  tradeData: AccountTradesDetailsModel[] = [];
  selectedPeriod: Period = Period.getDefault();
  currentDate: Date;
  private index = 1;

  private idClient: string;
  total: string[] = [];
  readonly cardType = TotalClientTotalsEnum;

  @ViewChild('trades', {read: ViewContainerRef, static: true}) tradesContainer: ViewContainerRef;
  displayedColumns: string[] = ['accountName', 'lastDay', 'mtd', 'lastMonth', 'avgDaily', 'mAvgDaily'];
  @ViewChild('table', {static: false}) table: MatTable<TotalClientsTableModel[]>;
  tableData: TotalClientsTableModel[] = [{
    id: 0,
    rowName: 'TOTAL TRADES',
    mtd: '0',
    lastDay: '0',
    avgDailyMonth: '0',
    avgDailyYear: '0',
    lastMonth: '0'
  },
    {
      id: 1,
      rowName: 'AVG TRADE SIZE',
      mtd: '0',
      lastDay: '0',
      avgDailyMonth: '0',
      avgDailyYear: '0',
      lastMonth: '0'
    }];

  listData = [
    [
      {name: 'NFLX', value: 800},
      {name: 'AMZN', value: 500},
      {name: 'Spain', value: 300},
    ],
    [
      {name: 'IAAPL', value: 800},
      {name: 'Goog', value: 500},
      {name: 'NFLX', value: 300},
    ],
  ];

  constructor(private route: ActivatedRoute,
              private resolver: ComponentFactoryResolver,
              private loading: LoadingService,
              private clientService: ClientsService,
              private clientsService: ClientsTradesService,
              private clientTradeService: ClientsTradesService) {
  }

  ngOnInit() {
    this.route.parent.params.subscribe(params => {
      this.idClient = params['id'];
      this.currentDate = this.selectedPeriod.toDate;
      this.resetFilters();
      this.refreshTotalData();
      this.refreshTable();
      this.refreshList();
      this.getAvailableTradeDates();
    });

  }

  tradeDateChanged(date: string) {
    this.selectedTradesStrDate = date;
    this.tradesPageIndex = 0;
    this.tradeDataLength = 0;
    this.currentTradesIndex = 1;
    this.clientsService.getTrades(date, this.idClient, this.tradesPageLength, this.tradesPageIndex)
      .subscribe(data => {
          this.tradeData = data.data;
          this.tradeDataLength = data.dataLength;
        },
        () => {
          this.tradeData = [];
          this.tradeDataLength = 0;
        });
  }

  getAvailableTradeDates() {
    this.clientsService.getAvailableTradeDates(this.idClient)
      .subscribe(
        data => {
          this.availableTradesDate = data.sort();
          if (data.length > 0) {
            this.tradeStartDate = this.availableTradesDate[this.availableTradesDate.length - 1];
          }
        },
        () => {
          this.availableTradesDate = [];
          this.tradeData = [];
        }
      );
  }

  private refreshTable() {
    this.loading.setLoading(true);
    this.clientTradeService.getTableData(this.idClient)
      .pipe(
        catchError(err => {
          return of(null);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data) {
          this.tableData = data;
          this.table.renderRows();
        }
      });
  }

  private refreshList() {
    this.loading.setLoading(true);
    this.clientTradeService.getList(this.idClient)
      .pipe(
        catchError(err => {
          return of(null);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data) {
          this.listData = data
            .map(value => {
              return value.accountTotalList.map(val => {
                return {
                  name: val.key,
                  value: val.value
                };
              });
            });
        }
      });
  }

  nextTrades(number: number = 1) {
    if (this.currentTradesIndex < this.tradeDataLength) {
      if (this.currentTradesIndex + number > this.tradeDataLength) {
        this.currentTradesIndex = this.tradeDataLength;
      } else {
        this.currentTradesIndex += number;
      }

      if (this.tradeData.length - this.currentTradesIndex <= 3) {
        this.loadNextOpenPos();
      }
    }
  }

  private loadNextOpenPos() {
    this.clientsService.getTrades(this.selectedTradesStrDate, this.idClient, this.tradesPageLength, this.tradesPageIndex + 1)
      .subscribe(data => {
          this.tradeData.push(...data.data);
          ++this.tradesPageIndex;
          this.tradeDataLength = data.dataLength;
        },
        () => {
          this.tradeData = [];
          this.tradeDataLength = 0;
        });
  }

  collapseTrades(number = -1) {
    if (number === -1 || this.currentTradesIndex - 1 <= 0) {
      this.currentTradesIndex = 1;
    } else {
      this.currentTradesIndex -= number;
    }
  }

  private resetFilters() {
    this.availableTradesDate = [];
    this.tradeData = [];
    this.tradeStartDate = null;
    this.selectedTradesStrDate = '';
    this.currentTradesIndex = 1;
    this.tradeDataLength = 0;
    this.tradesPageIndex = 0;
  }

  // clearTrades(): void {
  //   this.tradesContainer.clear();
  // }
  //
  // private isNextDate(): boolean {
  //   return !moment(this.currentDate).isSame(moment(this.selectedPeriod.fromDate), 'hours');
  // }
  //
  // private createNextTrade(): void {
  //   this.currentDate = this.nextDate();
  // //  this.createTrade();
  // }
  // private nextDate(): Date {
  //   return moment(this.currentDate).subtract(1, 'day').toDate();
  // }

  refreshTotalData() {
    this.loading.setLoading(true);
    this.clientTradeService.getTotalData(this.idClient)
      .pipe(
        catchError(err => {
          return of(null);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(value => {
        this.total = value;
      });
  }

}
