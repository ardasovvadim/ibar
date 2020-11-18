import { Injectable } from '@angular/core';
import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router} from '@angular/router';
import { Observable } from 'rxjs';
import {StorageService} from '../services/storage.service';

@Injectable()
export class RegGuard implements CanActivate {

  constructor(private storage: StorageService,
              private router: Router) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.storage.getFromRepository('registrationUser') != null) {
      return true;
    }
    this.router.navigate(['/']);
    return false;
  }

}
