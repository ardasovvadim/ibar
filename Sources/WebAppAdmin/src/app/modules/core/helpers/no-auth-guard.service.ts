import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {AuthenticationService} from '../services/authentication.service';
import {RouterService} from '../services/router.service';

@Injectable()
export class NoAuthGuard implements CanActivate {

  constructor(private authService: AuthenticationService,
              private router: RouterService) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (!this.authService.currentAdminUserValue) {
      return true;
    }
    this.router.navigate();
    return false;
  }

}
