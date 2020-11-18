import {Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {Overlay} from '@angular/cdk/overlay';
import {ComponentPortal} from '@angular/cdk/portal';
import {LoadingComponent} from '../../ui-common/components/loading/loading.component';

@Injectable()
export class LoadingService {

  private isLoadingSubject = new BehaviorSubject<boolean>(false);
  isLoading = this.isLoadingSubject.asObservable();
  private counterLoads = 0;

  constructor(private overlay: Overlay) {
  }

  create() {
    const positionStrategy = this.overlay.position()
      .global()
      .bottom('20px')
      .right('20px');

    const overlayRef = this.overlay.create({positionStrategy});

    const filePreviewPortal = new ComponentPortal(LoadingComponent);

    overlayRef.attach(filePreviewPortal);
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

    if (this.counterLoads === 0) {
      this.isLoadingSubject.next(false);
    } else {
      this.isLoadingSubject.next(true);
    }
  }

}
