import {Component, OnDestroy, OnInit} from '@angular/core';
import {BehaviourComponentService} from '../../../core/services/behaviour-component.service';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-button-navigation',
  templateUrl: './button-navigation.component.html',
  styleUrls: ['./button-navigation.component.sass']
})
export class ButtonNavigationComponent implements OnInit, OnDestroy {

  ngIf = false;
  isPrev = false;
  isNext = false;
  private sub: Subscription;

  constructor(private behaviourComponentService: BehaviourComponentService) {
  }

  ngOnInit() {
    this.sub = this.behaviourComponentService.navMenuButtonNavEvent.subscribe(event => {
      this.isNext = event.isNext;
      this.isPrev = event.isPrev;
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  return() {
    this.behaviourComponentService.emitReturnEvent();
  }

  prev() {
    this.behaviourComponentService.emitPrevEvent();
  }

  next() {
    this.behaviourComponentService.emitNextEvent();
  }

}
