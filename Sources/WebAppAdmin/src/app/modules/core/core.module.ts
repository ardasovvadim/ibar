import {NgModule} from '@angular/core';
import {AuthGuard} from './helpers/auth.guard';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {ErrorInterceptor} from './interceptors/error.interceptor';
import {JwtInterceptor} from './interceptors/jwt.interceptor';
import {ApiService} from './services/api.service';
import {AuthenticationService} from './services/authentication.service';
import {StorageService} from './services/storage.service';
import {NoAuthGuard} from './helpers/no-auth-guard.service';
import {ConfirmGuard} from './helpers/confirm.guard';
import {LoadingService} from './services/loading.service';
import {ConfirmService} from './services/confirm.service';
import {MasterAccountService} from '../ui-admin/pages/master-account/master-account.service';
import {UserService} from '../ui-admin/pages/user/user.service';
import {ExceptionHeaderService} from './services/exception-header.service';

@NgModule({
  imports: [
    HttpClientModule
  ],
  exports: [
    HttpClientModule
  ],
  providers: [
    ApiService,
    AuthGuard,
    NoAuthGuard,
    ConfirmGuard,
    LoadingService,
    AuthenticationService,
    StorageService,
    ConfirmService,
    MasterAccountService,
    UserService,
    ExceptionHeaderService,
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
  ]
})
export class CoreModule {
}
