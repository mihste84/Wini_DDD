import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../../environments/environment';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const apiReq = request.clone({
      url: environment.apiConfig.uri + request.url,
    });
    this.loadingService.startLoading();
    return next.handle(apiReq).pipe(
      delay(500),
      finalize(() => this.loadingService.stopLoading())
    );
  }
}
