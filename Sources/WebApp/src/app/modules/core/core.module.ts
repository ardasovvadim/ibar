import {NgModule} from '@angular/core';
import {ApiService} from './services/api.service';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {JwtInterceptor} from './interceptors/jwt.interceptor';
import {ErrorInterceptor} from './interceptors/error.interceptor';
import {AuthenticationService} from './services/authentication.service';
import {BehaviourComponentService} from './services/behaviour-component.service';
import {AuthGuard} from './helpers/auth.guard';
import {MasterAccountService} from './services/master-account.service';
import {TradeAccountService} from './services/trade-account.service';
import {RegistrationService} from './services/registration.service';
import {NoAuthGuard} from './helpers/no-auth.guard';
import {ConfirmRegGuard} from './helpers/confirm-reg-guard.service';
import {RegGuard} from './helpers/reg.guard';
import {LoadingService} from './services/loading.service';
import {DashboardService} from './services/dashboard.service';
import {FilterService} from './services/filter.service';
import {ExceptionHeaderService} from './services/exception-header.service';
import {RouterService} from './services/router.service';
import {UserService} from './services/user.service';


@NgModule({
  imports: [
    HttpClientModule
  ],
  exports: [
    HttpClientModule
  ],
  providers: [
    AuthGuard,
    NoAuthGuard,
    RegGuard,
    ConfirmRegGuard,
    ApiService,
    DashboardService,
    AuthenticationService,
    MasterAccountService,
    TradeAccountService,
    RegistrationService,
    LoadingService,
    FilterService,
    ExceptionHeaderService,
    RouterService,
    UserService,
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true}
  ]
})
export class CoreModule {
}
