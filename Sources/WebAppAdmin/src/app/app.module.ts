import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {UiCommonModule} from './modules/ui-common/ui-common.module';
import {RouterModule} from '@angular/router';
import {AppRoutingModule} from './app-routing.module';
import {CoreModule} from './modules/core/core.module';
import {RouterService} from './modules/core/services/router.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule,
    AppRoutingModule,
    UiCommonModule,
    CoreModule
  ],
  providers: [RouterService],
  bootstrap: [AppComponent]
})
export class AppModule {
}
