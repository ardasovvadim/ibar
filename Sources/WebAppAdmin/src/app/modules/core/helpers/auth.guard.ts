import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {AuthenticationService} from '../services/authentication.service';
import {RouterService} from '../services/router.service';

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private router: RouterService,
              private authenticationService: AuthenticationService) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const currentAdminUser = this.authenticationService.currentAdminUserValue;
    if (currentAdminUser) {
      return true;
    }
    this.router.navigate('/login');
    return false;
  }

}
