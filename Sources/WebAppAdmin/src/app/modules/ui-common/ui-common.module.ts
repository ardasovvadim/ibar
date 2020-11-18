import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {HeaderComponent} from './components/header/header.component';
import {FooterComponent} from './components/footer/footer.component';
import {AngularMaterialModule} from './angular-material.module';
import {RouterModule} from '@angular/router';
import {LoadingComponent} from './components/loading/loading.component';
import {CustomPaginationComponent} from './components/custom-pagination/custom-pagination.component';
import {ConfirmDialogComponent} from './components/confirm-dialog/confirm-dialog.component';
import {CheckEmptyPipe} from './pipes/check-empty.pipe';
import {StateModelErrorsComponent} from './components/state-model-errors/state-model-errors.component';
import {ExceptionHeaderComponent} from './components/exception-header/exception-header.component';
import { MatBadgeModule } from '@angular/material/badge';
import { MatRippleModule } from '@angular/material/core';
import { DateRangeComponent } from './components/date-range/date-range.component';
import { CustomDateTimePipe } from './pipes/custom-date-time.pipe';
import { CustomDatePipe } from './pipes/custom-date.pipe';

@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    LoadingComponent,
    CustomPaginationComponent,
    ConfirmDialogComponent,
    CheckEmptyPipe,
    StateModelErrorsComponent,
    ExceptionHeaderComponent,
    DateRangeComponent,
    CustomDateTimePipe,
    CustomDatePipe,
  ],
  exports: [
    FooterComponent,
    HeaderComponent,
    AngularMaterialModule,
    CustomPaginationComponent,
    CheckEmptyPipe,
    StateModelErrorsComponent,
    ExceptionHeaderComponent,
    DateRangeComponent,
    CustomDateTimePipe,
    CustomDatePipe,
    LoadingComponent,
  ],
  imports: [
    CommonModule,
    AngularMaterialModule,
    RouterModule,
    MatBadgeModule,
    MatRippleModule,
  ],
  entryComponents: [
    LoadingComponent,
    ConfirmDialogComponent,
  ]
})
export class UiCommonModule {
}

