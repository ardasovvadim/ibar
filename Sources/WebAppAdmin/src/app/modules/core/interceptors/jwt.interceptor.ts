import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {AuthenticationService} from '../services/authentication.service';
import {Observable} from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const currentAdminUser = this.authenticationService.currentAdminUserValue;
    if (currentAdminUser && currentAdminUser.accessToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentAdminUser.accessToken}`
        }
      });
    } else {
      request = request.clone({
        setHeaders: {
          'Access-Control-Allow-Origin': '*'
        }
      });
    }

    return next.handle(request);
  }
}
