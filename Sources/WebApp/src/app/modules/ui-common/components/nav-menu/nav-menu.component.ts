import {ChangeDetectorRef, Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {UserModel} from '../../../core/models/user.model';
import {BehaviourComponentService} from '../../../core/services/behaviour-component.service';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {MasterAccountModel} from '../../../core/models/master-account.model';
import {Period} from '../../../core/models/period';
import {TradeAccountModel} from '../../../core/models/trade-account.model';
import {AccountPickerComponent} from '../account-picker/account-picker.component';
import {TradeAccountSearchBoxComponent} from '../trade-account-search-box/trade-account-search-box.component';
import {FilterService} from '../../../core/services/filter.service';
import {MatDialog} from '@angular/material/dialog';
import {ChangePasswordDialogComponent} from '../../pages/change-password-dialog/change-password-dialog.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.sass']
})
export class NavMenuComponent implements OnInit {

  // ---
  user: UserModel = null;
  isSearch: boolean;
  // ---
  isMasterAccountPicker: boolean;
  isRangeDatepicker: boolean;
  pageTitle: string;
  // ---
  @ViewChild('tradeAccountSearchBox', {static: false}) tradeAccountSearchBox: TradeAccountSearchBoxComponent;
  @ViewChild('masterPicker', {static: false}) masterPicker: AccountPickerComponent;
  isButtonNavigation = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private behaviourComponentService: BehaviourComponentService,
              private authenticationService: AuthenticationService,
              private filterService: FilterService,
              private cdRef: ChangeDetectorRef,
              private dialog: MatDialog) {
  }

  ngOnInit() {
    this.authenticationService.currentUser.subscribe(user => {
      this.user = user;
    });
    this.behaviourComponentService.isNavSearch.subscribe(value => {
      this.isSearch = value;
      this.cdRef.detectChanges();
    });
    this.behaviourComponentService.isNavMenuMasterAccPic.subscribe(value => {
      this.isMasterAccountPicker = value;
      this.cdRef.detectChanges();
    });
    this.behaviourComponentService.isNavMenuRangeDatepic.subscribe(value => {
      this.isRangeDatepicker = value;
      this.cdRef.detectChanges();
    });
    this.behaviourComponentService.pageTitle.subscribe(value => {
      this.pageTitle = value;
      this.cdRef.detectChanges();
    });
    this.behaviourComponentService.isNavMenuButtonNavigation.subscribe(value => {
      this.isButtonNavigation = value;
      this.cdRef.detectChanges();
    });
  }

  searchNameChange(tradeAccount: TradeAccountModel) {
    if (tradeAccount == null) {
    } else {
      this.filterService.setSelectedTradeAccounts([tradeAccount]);
      this.masterPicker.setDefault();
    }
  }

  logOut() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  selectedMasterAccountChange(masterAccount: MasterAccountModel) {
    this.tradeAccountSearchBox.searchName = null;
    this.filterService.setSelectedMasterAccounts([masterAccount]);
  }

  selectedPeriodChange(period: Period) {
    this.filterService.setSelectedPeriod(period);
  }

  setDefaultMasterAccount() {
    this.filterService.setSelectedMasterAccounts([MasterAccountModel.getDefault()]);
  }

  openChangePasswordDialog() {
   this.dialog.open(ChangePasswordDialogComponent)
     .afterClosed()
     .subscribe();
  }
}

