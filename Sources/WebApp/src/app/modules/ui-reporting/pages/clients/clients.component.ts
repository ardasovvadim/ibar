import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BehaviourComponentService} from '../../../core/services/behaviour-component.service';
import {Subscription} from 'rxjs';
import {AccountInfoService} from '../recent-accounts/account-info.service';
import {ClientsService} from './clients.service';
import {LoadingService} from '../../../core/services/loading.service';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.sass']
})
export class ClientsComponent implements OnInit, OnDestroy {

  subscriptions: Subscription[] = [];

  constructor(private route: ActivatedRoute,
              private router: Router,
              private activatedRoute: ActivatedRoute,
              private behaviourComponentService: BehaviourComponentService,
              private accountInfoService: AccountInfoService,
              private clientsService: ClientsService,
              private loading: LoadingService) {
  }

  ngOnInit(): void {
    // --> Settings header
    this.behaviourComponentService.setNavSearch(false);
    this.behaviourComponentService.setNavMenuMasterAccPic(false);
    this.behaviourComponentService.setNavMenuRangeDatepic(false);
    this.behaviourComponentService.setNavMenuButtonNavigationSource(true);
    // ---

    // --> Setting account header
    let sub = this.route.params.subscribe(params => {
      const idParam = params['id'];
      this.clientsService.setAccountHeader({id: idParam, name: ''});
    });
    this.subscriptions.push(sub);
    // ---

    // --> Navigation buttons
    sub = this.behaviourComponentService.returnEvent.subscribe(() => this.router.navigate(['/recent-accounts']));
    this.subscriptions.push(sub);
    sub = this.behaviourComponentService.prevEvent.subscribe(() => {
      if (!this.loading.isLoadingValue) {
        if (this.accountInfoService.isPrev()) {
          const obj = this.accountInfoService.getPrev();
          const pagePath = this.route.firstChild.snapshot.url.pop().path;
          this.router.navigate([`/clients/${obj.name}/${pagePath}`]);
        }
      }
    });
    this.subscriptions.push(sub);
    sub = this.behaviourComponentService.nextEvent.subscribe(() => {
      if (!this.loading.isLoadingValue) {
        if (this.accountInfoService.isNext()) {
          const obj = this.accountInfoService.getNext();
          console.log(this.route.children.pop().url);
          const pagePath = this.route.firstChild.snapshot.url.pop().path;
          this.router.navigate([`/clients/${obj.name}/${pagePath}`]);
        }
      }
    });
    this.subscriptions.push(sub);
    // ---
  }

  ngOnDestroy(): void {
    // --> Setting header
    this.behaviourComponentService.setNavSearch(true);
    this.behaviourComponentService.setNavMenuMasterAccPic(true);
    this.behaviourComponentService.setNavMenuRangeDatepic(true);
    this.behaviourComponentService.setNavMenuButtonNavigationSource(false);
    this.subscriptions.forEach(sub => sub.unsubscribe());
    // --
  }

}
