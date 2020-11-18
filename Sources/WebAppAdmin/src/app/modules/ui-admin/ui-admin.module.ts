import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LoginComponent} from './pages/login/login.component';
import {UiCommonModule} from '../ui-common/ui-common.module';
import {UiAdminRoutingModule} from './ui-admin-routing.module';
import {UserComponent} from './pages/user/user.component';
import {UserAddDialogComponent} from './pages/user/user-add-dialog/user-add-dialog.component';
import {MasterAccountComponent} from './pages/master-account/master-account.component';
import {MasterAccountAddDialogComponent} from './pages/master-account/master-account-add-dialog/master-account-add-dialog.component';
import {UserEditDialogComponent} from './pages/user/user-edit-dialog/user-edit-dialog.component';
import {MasterAccountEditDialogComponent} from './pages/master-account/master-account-edit-dialog/master-account-edit-dialog.component';
import {LoginConfirmComponent} from './pages/login/login-confirm/login-confirm.component';
import {SourcesComponent} from './pages/sources/sources.component';
import {ControlsComponent} from './pages/controls/controls.component';
import {JobControlComponent} from './pages/controls/job-control/job-control.component';
import {ExceptionComponent} from './pages/exception/exception.component';
import {SettingsComponent} from './pages/settings/settings.component';
import {FtpCredentialComponent} from './pages/settings/ftp-credential/ftp-credential.component';
import {FtpCredentialPanelComponent} from './pages/settings/ftp-credential/ftp-credential-panel/ftp-credential-panel.component';
import {FtpCredentialAddDialogComponent} from './pages/settings/ftp-credential/ftp-credential-add-dialog/ftp-credential-add-dialog.component';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import {CustomErrorStateMatcher} from '../core/helpers/custom-error-state-matcher';
import { AnalyticsPageComponent } from './pages/analytics-page/analytics-page.component';
import { SelectMasterAccountsComponent } from './components/select-master-accounts/select-master-accounts.component';
import { AnalyticsChartComponent } from './pages/analytics-page/analytics-chart/analytics-chart.component';
import {NgApexchartsModule} from 'ng-apexcharts';
import { SelectFtpCredentialsComponent } from './components/select-ftp-credentials/select-ftp-credentials.component';


@NgModule({
  declarations: [
    LoginComponent,
    UserComponent,
    UserAddDialogComponent,
    MasterAccountComponent,
    MasterAccountAddDialogComponent,
    UserEditDialogComponent,
    MasterAccountEditDialogComponent,
    LoginConfirmComponent,
    SourcesComponent,
    ControlsComponent,
    JobControlComponent,
    ExceptionComponent,
    SettingsComponent,
    FtpCredentialComponent,
    FtpCredentialPanelComponent,
    FtpCredentialAddDialogComponent,
    AnalyticsPageComponent,
    SelectMasterAccountsComponent,
    AnalyticsChartComponent,
    SelectFtpCredentialsComponent,
  ],
  imports: [
    CommonModule,
    UiCommonModule,
    UiAdminRoutingModule,
    MatIconModule,
    NgApexchartsModule,
  ],
  entryComponents: [
    UserAddDialogComponent,
    UserEditDialogComponent,
    MasterAccountAddDialogComponent,
    MasterAccountEditDialogComponent,
    FtpCredentialAddDialogComponent
  ],
  providers: [
    {provide: ErrorStateMatcher, useClass: CustomErrorStateMatcher}
  ]
})
export class UiAdminModule {
}
