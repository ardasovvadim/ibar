import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from '../core/helpers/auth.guard';
import {LoginComponent} from './pages/login/login.component';
import {UserComponent} from './pages/user/user.component';
import {MasterAccountComponent} from './pages/master-account/master-account.component';
import {NoAuthGuard} from '../core/helpers/no-auth-guard.service';
import {LoginConfirmComponent} from './pages/login/login-confirm/login-confirm.component';
import {ConfirmGuard} from '../core/helpers/confirm.guard';
import {SourcesComponent} from './pages/sources/sources.component';
import {ExceptionComponent} from './pages/exception/exception.component';
import {SettingsComponent} from './pages/settings/settings.component';
import {AnalyticsPageComponent} from './pages/analytics-page/analytics-page.component';

const routes: Routes = [
  {path: '', redirectTo: 'users', pathMatch: 'full'},
  {path: 'users', component: UserComponent, canActivate: [AuthGuard]},
  {path: 'masteraccounts', component: MasterAccountComponent, canActivate: [AuthGuard]},
  {path: 'sources', component: SourcesComponent, canActivate: [AuthGuard]},
  // {path: 'controls', component: ControlsComponent, canActivate: [AuthGuard]},
  {path: 'exception/{id}', component: ExceptionComponent, canActivate: [AuthGuard]},
  {path: 'analytics', component: AnalyticsPageComponent, canActivate: [AuthGuard]},
  {path: 'settings', component: SettingsComponent, canActivate: [AuthGuard]},

  {path: 'login', component: LoginComponent, canActivate: [NoAuthGuard]},
  {path: 'login/confirm', component: LoginConfirmComponent, canActivate: [ConfirmGuard]}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UiAdminRoutingModule {
}
