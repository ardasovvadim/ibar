import {NgModule} from '@angular/core';
import {CommonModule, CurrencyPipe} from '@angular/common';
import {BarChartComponent} from './components/charts/bar-chart/bar-chart.component';
import {DashboardComponent} from './pages/dashboard/dashboard.component';
import {IncomeComponent} from './pages/income/income.component';
import {IncomeBlockComponent} from './pages/income/income-block/income-block.component';
import {IncomePieBlockComponent} from './pages/income/income-pie-block/income-pie-block.component';
import {UiCommonModule} from '../ui-common/ui-common.module';
import {TotalClientsComponent} from './pages/total-clients/total-clients.component';
import {ListComponent} from './pages/total-clients/list/list.component';
import {ClientPortfolioComponent} from './pages/clients/client-portfolio/client-portfolio.component';
import {ClientNavComponent} from './pages/clients/client-nav/client-nav.component';
import {ClientsComponent} from './pages/clients/clients.component';
import {ClientTradesComponent} from './pages/clients/client-trades/client-trades.component';
import {TradeBlockComponent} from './pages/clients/client-trades/trade-block/trade-block.component';
import {PorfolioBlockComponent} from './pages/clients/client-portfolio/porfolio-block/porfolio-block.component';
import {UiReportingRoutingModule} from './ui-reporting-routing.module';
import {AccountDetailsComponent} from './pages/clients/client-details/account-details.component';
import {NoteComponent} from './pages/clients/client-details/note/note.component';
import {NoteAddDialogComponent} from './pages/clients/client-details/note-add-dialog/note-add-dialog.component';
import {NewClientsChartComponent} from './pages/recent-accounts/new-clients-chart/new-clients-chart.component';
import {RecentAccountsComponent} from './pages/recent-accounts/recent-accounts.component';
import {ErrorStateMatcher} from '@angular/material/core';
import {CustomErrorStateMatcher} from '../core/helpers/custom-error-state-matcher';
import {PortfolioLineChartComponent} from './pages/clients/client-portfolio/portfolio-line-chart/portfolio-line-chart.component';
import { OpenPositionBlockComponent } from './pages/clients/client-portfolio/open-position-block/open-position-block.component';
import {CustomCurrencyPipe} from '../ui-common/pipes/custom-currency.pipe';


@NgModule({
  declarations: [
    BarChartComponent,
    AccountDetailsComponent,
    NoteComponent,
    DashboardComponent,
    IncomeComponent,
    IncomeBlockComponent,
    IncomePieBlockComponent,
    RecentAccountsComponent,
    TotalClientsComponent,
    ListComponent,
    ClientPortfolioComponent,
    ClientNavComponent,
    ClientsComponent,
    ClientTradesComponent,
    TradeBlockComponent,
    PorfolioBlockComponent,
    NewClientsChartComponent,
    NoteAddDialogComponent,
    PortfolioLineChartComponent,
    OpenPositionBlockComponent
  ],
  imports: [
    UiReportingRoutingModule,
    UiCommonModule,
    CommonModule,
  ],
  exports: [],
  entryComponents: [
    TradeBlockComponent,
    PorfolioBlockComponent,
    NoteAddDialogComponent,
  ],
  providers: [
    {provide: ErrorStateMatcher, useClass: CustomErrorStateMatcher},
    CurrencyPipe,
    CustomCurrencyPipe
  ]
})
export class UiReportingModule {
}
