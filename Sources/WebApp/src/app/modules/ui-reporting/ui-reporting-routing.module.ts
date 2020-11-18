import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LoginComponent} from '../ui-common/pages/login/login.component';
import {NoAuthGuard} from '../core/helpers/no-auth.guard';
import {LoginConfirmComponent} from '../ui-common/pages/login/login-confirm/login-confirm.component';
import {RegistrationConfirmComponent} from '../ui-common/pages/registration/registration-confirm/registration-confirm.component';
import {ConfirmRegGuard} from '../core/helpers/confirm-reg-guard.service';
import {RegistrationComponent} from '../ui-common/pages/registration/registration.component';
import {RegGuard} from '../core/helpers/reg.guard';
import {DashboardComponent} from './pages/dashboard/dashboard.component';
import {AuthGuard} from '../core/helpers/auth.guard';
import {ClientsComponent} from './pages/clients/clients.component';
import {ClientPortfolioComponent} from './pages/clients/client-portfolio/client-portfolio.component';
import {ClientTradesComponent} from './pages/clients/client-trades/client-trades.component';
import {IncomeComponent} from './pages/income/income.component';
import {TotalClientsComponent} from './pages/total-clients/total-clients.component';
import {AccountDetailsComponent} from './pages/clients/client-details/account-details.component';
import {RecentAccountsComponent} from './pages/recent-accounts/recent-accounts.component';

const routes: Routes = [
  {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
  {path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard]},
  {
    path: 'clients/:id', component: ClientsComponent, canActivate: [AuthGuard], children: [
      {path: '', redirectTo: 'account', pathMatch: 'full', canActivate: [AuthGuard]},
      {path: 'account', component: AccountDetailsComponent, canActivate: [AuthGuard]},
      {path: 'portfolio', component: ClientPortfolioComponent, canActivate: [AuthGuard]},
      {path: 'trades', component: ClientTradesComponent, canActivate: [AuthGuard]},
    ]
  },
  {path: 'income', component: IncomeComponent, canActivate: [AuthGuard]},
  {path: 'recent-accounts', component: RecentAccountsComponent, canActivate: [AuthGuard]},
  {path: 'totalAccounts', component: TotalClientsComponent, canActivate: [AuthGuard]},
  // Auth routes
  {path: 'login', component: LoginComponent, canActivate: [NoAuthGuard]},
  {path: 'login/confirm', component: LoginConfirmComponent, canActivate: [NoAuthGuard]},
  // Registration routes
  {path: 'registration/confirm/:linkKey', component: RegistrationConfirmComponent, canActivate: [ConfirmRegGuard]},
  {path: 'registration', component: RegistrationComponent, canActivate: [RegGuard, ConfirmRegGuard]},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UiReportingRoutingModule {
}
