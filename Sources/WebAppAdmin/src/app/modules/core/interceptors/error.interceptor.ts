import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {AuthenticationService} from '../services/authentication.service';
import {Observable, of, throwError} from 'rxjs';
import {catchError} from 'rxjs/operators';
import {RouterService} from '../services/router.service';
import {ExceptionHeaderService} from '../services/exception-header.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService,
              private router: RouterService,
              private exceptionService: ExceptionHeaderService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(error => {
      switch (error.status) {
        case 0: {
          this.exceptionService.nextException({messages: ['Error connection refused']});
          break;
        }
        case 401: {
          this.authenticationService.logout();
          this.router.navigate('/login');
          break;
        }
        case 400: {
          break;
        }
        case 500: {
          if (typeof error.error === 'string') {
            this.exceptionService.nextException({guid: error.error, isReportButton: true});
          } else {
            this.exceptionService.nextException({messages: ['Unrecognized error']});
          }
          break;
        }
        case 404: {
          this.exceptionService.nextException({messages: ['Not found']});
          break;
        }
        default: {
          this.exceptionService.nextException({messages: ['Unrecognized error']});
          break;
        }
      }

      return throwError(error);
    }));
  }
}
