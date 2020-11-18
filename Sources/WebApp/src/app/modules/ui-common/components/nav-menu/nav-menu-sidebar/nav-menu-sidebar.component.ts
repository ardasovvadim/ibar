import {AfterViewInit, Component, ElementRef, OnInit, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {ExceptionHeaderService} from '../../../../core/services/exception-header.service';
import {BehaviourComponentService} from '../../../../core/services/behaviour-component.service';

@Component({
  selector: 'app-nav-menu-sidebar',
  templateUrl: './nav-menu-sidebar.component.html',
  styleUrls: ['./nav-menu-sidebar.component.sass']
})
export class NavMenuSidebarComponent implements OnInit {

  readonly urlDashboard = '/dashboard';
  readonly urlIncome = '/income';
  readonly urlRecentAccounts = '/recent-accounts';
  readonly urlSales = '/sales';
  readonly urlTotalAccounts = '/totalAccounts';
  readonly urlAccount = '/clients';
  // TODO: ardasovvadim: temporary for navigation
  idClient = 1;
  // ---
  menuIsActive = true;
  @ViewChild('searchInput', {static: false, read: ElementRef}) searchInput: ElementRef;

  constructor(private behaviourComponentService: BehaviourComponentService) {
  }

  ngOnInit() {
  }

  onClick() {
    this.menuIsActive = !this.menuIsActive;
  }

  onClickSearch() {
    this.onClick();
    this.searchInput.nativeElement.focus();
  }

  closeMenu() {
    this.onClick();
  }
}
