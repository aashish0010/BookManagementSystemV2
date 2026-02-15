import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  public redirectUrl: string | undefined;
  public confirmed: boolean = false;
  public isLogin: boolean = false;

  login(payload: { email: string; password: string }): Observable<any> {
    return this.http.post(`${environment.baseURL}auth/login`, {
      userName: payload.email,
      password: payload.password,
    });
  }

  register(payload: {
    name: string;
    email: string;
    phone: number;
    password: string;
    country_code: number;
  }): Observable<any> {
    return this.http.post(`${environment.baseURL}auth/register`, {
      userName: payload.name,
      email: payload.email,
      phoneNumber: String(payload.phone),
      password: payload.password,
      companyCode: environment.companyCode,
    });
  }
}
