import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { type } from 'os';
import { Key } from 'protractor';

@Injectable()
// Handles errors returned from the API that is stored in the header
export class ErrorIntercepter implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse) {
          if (error.status === 401) {
            return throwError(error.statusText);
          }

          const applicationError = error.headers.get('Application-Error');
          if (applicationError) {
            console.error(applicationError);
            return throwError(applicationError);
          }

          const serverError = error.error;
          let modalStateError = '';
          if (serverError && typeof serverError === 'object') {
            for (const key in serverError) {
              if (serverError[key]) {
                modalStateError += serverError[key] + '\n';
              }
            }
          }

          return throwError(modalStateError || serverError || 'Server Error');
        }
      })
    );
  }
}

// used in calling the intercepter class as a provider
export const ErrorIntercepterProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorIntercepter,
  multi: true
};
