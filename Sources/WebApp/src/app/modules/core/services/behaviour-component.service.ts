import {EventEmitter, Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BehaviourComponentService {

  private isNavSearchSource = new BehaviorSubject<boolean>(true);
  isNavSearch = this.isNavSearchSource.asObservable();

  private isNavMenuMasterAccPicSource = new BehaviorSubject<boolean>(true);
  isNavMenuMasterAccPic = this.isNavMenuMasterAccPicSource.asObservable();

  private isNavMenuRangeDatepicSource = new BehaviorSubject<boolean>(true);
  isNavMenuRangeDatepic = this.isNavMenuRangeDatepicSource.asObservable();

  private pageTitleSource = new BehaviorSubject<string>(null);
  pageTitle = this.pageTitleSource.asObservable();

  private isNavMenuButtonNavigationSource = new BehaviorSubject<boolean>(false);
  isNavMenuButtonNavigation = this.isNavMenuButtonNavigationSource.asObservable();

  private navMenuButtonNavSource = new BehaviorSubject<{isPrev: boolean, isNext: boolean}>({isPrev: false, isNext: false});
  navMenuButtonNavEvent = this.navMenuButtonNavSource.asObservable();

  returnEvent = new EventEmitter();
  prevEvent = new EventEmitter();
  nextEvent = new EventEmitter();

  constructor() {
  }

  emitNavMenuButtonNavEvent(event: {isPrev: boolean, isNext: boolean}): void {
    this.navMenuButtonNavSource.next(event);
  }

  setNavSearch(value: boolean): void {
    this.isNavSearchSource.next(value);
  }

  setNavMenuMasterAccPic(value: boolean): void {
    this.isNavMenuMasterAccPicSource.next(value);
  }

  setNavMenuRangeDatepic(value: boolean): void {
    this.isNavMenuRangeDatepicSource.next(value);
  }

  setPageTitle(value?: string): void {
    this.pageTitleSource.next(value);
  }

  setNavMenuButtonNavigationSource(value: boolean): void {
    this.isNavMenuButtonNavigationSource.next(value);
  }

  reset() {
    this.setNavSearch(true);
    this.setNavMenuMasterAccPic(true);
    this.setNavMenuRangeDatepic(true);
    this.setPageTitle(null);
  }

  emitReturnEvent = () => this.returnEvent.emit();

  emitPrevEvent = () => this.prevEvent.emit();

  emitNextEvent = () => this.nextEvent.emit();
}
