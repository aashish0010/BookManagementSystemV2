import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable()
export class TokenmanagerInterceptor implements HttpInterceptor {

  constructor(public route: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    var token = localStorage.getItem("accesstoken");
    if(token!=null)
    {
      request.clone({ setHeaders: { authorization: `Bearer ${token}`  }});
    }
    return next.handle(request);
  }
}
