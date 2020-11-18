import {Injectable} from '@angular/core';
import {ComponentPortal} from '@angular/cdk/portal';
import {LoadingComponent} from '../../ui-common/components/loading/loading.component';
import {Overlay} from '@angular/cdk/overlay';
import {BehaviorSubject} from 'rxjs';

@Injectable()
export class LoadingService {

  private isLoadingSubject = new BehaviorSubject<boolean>(false);
  isLoading = this.isLoadingSubject.asObservable();
  private counterLoads = 0;

  constructor(private overlay: Overlay) {
  }

  get isLoadingValue(): boolean {
    return this.isLoadingSubject.value;
  }

  setLoading(value: boolean): void {
    if (value) {
      ++this.counterLoads;
    } else {
      --this.counterLoads;
    }

    if (this.counterLoads < 0) {
      this.counterLoads = 0;
    }

    if (this.counterLoads === 0) {
      this.isLoadingSubject.next(false);
    } else {
      this.isLoadingSubject.next(true);
    }
  }

}
