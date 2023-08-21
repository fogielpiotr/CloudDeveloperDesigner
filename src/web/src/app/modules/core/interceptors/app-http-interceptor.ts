import { MessageService } from 'primeng/api';
import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { catchError, finalize, Observable, tap, throwError } from 'rxjs';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable()
export class AppHttpInterceptor implements HttpInterceptor {

  constructor(private messageService: MessageService, private spinner: NgxSpinnerService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (request.url.indexOf("graph.microsoft.com") > -1) {
      return next.handle(request);
    }
    this.spinner.show();
    return next.handle(request).pipe(
      finalize(() => { this.spinner.hide() }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          const httpError = err as HttpErrorResponse;
          if (httpError.status == 0 ) {
            this.messageService.add({ severity: 'error', summary: 'A server error occurred. Please contact the Administrator.', key: 'httpErrorToast' });
          }
          else {
            this.messageService.add({ severity: 'error', summary: httpError.error.message, key: 'httpErrorToast' });
          }
        }
        this.spinner.hide();
        return throwError(() => err);
      })
    );
  }
}