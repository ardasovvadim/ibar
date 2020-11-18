import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {StorageService} from '../services/storage.service';
import {RouterService} from '../services/router.service';

@Injectable()
export class ConfirmGuard implements CanActivate {

  constructor(private storage: StorageService,
              private router: RouterService) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.storage.getFromRepository('confirmUser')) {
      return true;
    }
    this.router.navigate();
    return false;
  }

}
