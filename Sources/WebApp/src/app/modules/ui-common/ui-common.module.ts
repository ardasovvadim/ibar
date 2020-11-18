import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AccountPickerComponent} from './components/account-picker/account-picker.component';
import {PeriodPickerComponent} from './components/period-picker/period-picker.component';
import {FooterComponent} from './components/footer/footer.component';
import {NavMenuComponent} from './components/nav-menu/nav-menu.component';
import {NavMenuSidebarComponent} from './components/nav-menu/nav-menu-sidebar/nav-menu-sidebar.component';
import {RouterModule} from '@angular/router';
import {NgbDatepickerModule, NgbDropdownModule} from '@ng-bootstrap/ng-bootstrap';
import {FormsModule} from '@angular/forms';
import {CardComponent} from './components/card/card.component';
import {PieBlockComponent} from './components/pie-block/pie-block.component';
import {ChartsModule} from 'ng2-charts';
import {LoginComponent} from './pages/login/login.component';
import {RegistrationComponent} from './pages/registration/registration.component';
import {RegistrationConfirmComponent} from './pages/registration/registration-confirm/registration-confirm.component';
import {LoginConfirmComponent} from './pages/login/login-confirm/login-confirm.component';
import {LoadingComponent} from './components/loading/loading.component';
import {TradeAccountSearchBoxComponent} from './components/trade-account-search-box/trade-account-search-box.component';
import {CustomPaginationComponent} from './components/custom-pagination/custom-pagination.component';
import {DateToStringPipe} from './pipes/date-to-string.pipe';
import {DatetimeToStringPipe} from './pipes/datetime-to-string.pipe';
import {AngularMaterialModule} from './angular-material.module';
import {ExceptionHeaderComponent} from './components/exception-header/exception-header.component';
import {StateModelErrorsComponent} from './components/state-model-errors/state-model-errors.component';
import {ButtonNavigationComponent} from './components/button-navigation/button-navigation.component';
import {CheckEmptyPipe} from './pipes/check-empty.pipe';
import {DatePickerComponent} from './components/date-picker/date-picker.component';
import {CustomCurrencyPipe} from './pipes/custom-currency.pipe';
import {MatMenuModule} from '@angular/material/menu';
import { ChangePasswordDialogComponent } from './pages/change-password-dialog/change-password-dialog.component';
import {ErrorStateMatcher} from '@angular/material/core';
import {CustomErrorStateMatcher} from '../core/helpers/custom-error-state-matcher';

@NgModule({
  declarations: [
    CardComponent,
    AccountPickerComponent,
    PeriodPickerComponent,
    FooterComponent,
    NavMenuComponent,
    NavMenuSidebarComponent,
    PieBlockComponent,
    LoginComponent,
    RegistrationComponent,
    RegistrationConfirmComponent,
    LoginConfirmComponent,
    LoadingComponent,
    TradeAccountSearchBoxComponent,
    CustomPaginationComponent,
    DateToStringPipe,
    DatetimeToStringPipe,
    LoginComponent,
    LoginConfirmComponent,
    RegistrationComponent,
    RegistrationConfirmComponent,
    ExceptionHeaderComponent,
    StateModelErrorsComponent,
    ButtonNavigationComponent,
    CheckEmptyPipe,
    DatePickerComponent,
    CustomCurrencyPipe,
    ChangePasswordDialogComponent,
  ],
  exports: [
    CardComponent,
    AccountPickerComponent,
    PeriodPickerComponent,
    FooterComponent,
    NavMenuComponent,
    NavMenuSidebarComponent,
    PieBlockComponent,
    CustomPaginationComponent,
    DateToStringPipe,
    DatetimeToStringPipe,
    AngularMaterialModule,
    ChartsModule,
    FormsModule,
    NgbDatepickerModule,
    NgbDropdownModule,
    ExceptionHeaderComponent,
    CheckEmptyPipe,
    DatePickerComponent,
  ],
  imports: [
    AngularMaterialModule,
    CommonModule,
    RouterModule,
    NgbDatepickerModule,
    NgbDropdownModule,
    FormsModule,
    ChartsModule,
    MatMenuModule
  ],
  providers: [
    {provide: ErrorStateMatcher, useClass: CustomErrorStateMatcher},
  ],
  entryComponents: [
    LoadingComponent,
    ChangePasswordDialogComponent
  ]
})
export class UiCommonModule {
}
