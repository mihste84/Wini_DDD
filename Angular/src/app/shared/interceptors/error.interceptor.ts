import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { ErrorHandlerService } from '../services/error-handler.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private errorHandler: ErrorHandlerService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<any> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        this.errorHandler.addError(error);

        return throwError(() => error);
      })
    );
  }
}
