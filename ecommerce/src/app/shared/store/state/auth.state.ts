import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { Action, Selector, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs';

import { IAuthNumberLoginState } from '../../interface/auth.interface';
import { AuthService } from '../../services/auth.service';
import { AccountClearAction, GetUserDetailsAction } from '../action/account.action';
import {
  AuthClearAction,
  ForgotPasswordAction,
  LoginAction,
  LoginWithNumberAction,
  LogoutAction,
  RegisterAction,
  UpdatePasswordAction,
  VerifyNumberOTPAction,
  VerifyOTPAction,
} from '../action/auth.action';
import { ClearCartAction } from '../action/cart.action';

export interface AuthStateModel {
  email: String;
  number: IAuthNumberLoginState | null;
  token: String | Number;
  access_token: String | null;
  permissions: [];
}

@State<AuthStateModel>({
  name: 'auth',
  defaults: {
    email: '',
    token: '',
    number: null,
    access_token: '',
    permissions: [],
  },
})
@Injectable()
export class AuthState {
  private store = inject(Store);
  router = inject(Router);
  private authService = inject(AuthService);

  @Selector()
  static accessToken(state: AuthStateModel): String | null {
    return state.access_token;
  }

  @Selector()
  static isAuthenticated(state: AuthStateModel): Boolean {
    return !!state.access_token;
  }

  @Selector()
  static email(state: AuthStateModel): String {
    return state.email;
  }

  @Selector()
  static number(state: AuthStateModel): IAuthNumberLoginState | null {
    return state.number;
  }

  @Selector()
  static token(state: AuthStateModel): String | Number {
    return state.token;
  }

  @Action(RegisterAction)
  register(ctx: StateContext<AuthStateModel>, action: RegisterAction) {
    return this.authService.register(action.payload).pipe(
      tap({
        next: () => {
          void this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Registration failed', err);
        },
      }),
    );
  }

  @Action(LoginAction)
  login(ctx: StateContext<AuthStateModel>, action: LoginAction) {
    return this.authService.login(action.payload).pipe(
      tap({
        next: (res: any) => {
          ctx.patchState({
            email: action.payload.email,
            token: res.token || '',
            access_token: res.token || '',
          });
          this.store.dispatch(new GetUserDetailsAction());
        },
        error: (err) => {
          console.error('Login failed', err);
        },
      }),
    );
  }

  @Action(LoginWithNumberAction)
  loginWithNumber(_ctx: StateContext<AuthStateModel>, _action: LoginWithNumberAction) {
    // Login with number logic here
    this.store.dispatch(new GetUserDetailsAction());
  }

  @Action(ForgotPasswordAction)
  forgotPassword(_ctx: StateContext<AuthStateModel>, _action: ForgotPasswordAction) {
    // Forgot Password Logic Here
  }

  @Action(VerifyOTPAction)
  verifyEmail(_ctx: StateContext<AuthStateModel>, _action: VerifyOTPAction) {
    // Verify Logic Here
  }

  @Action(VerifyNumberOTPAction)
  verifyNumber(_ctx: StateContext<AuthStateModel>, _action: VerifyNumberOTPAction) {
    // Verify Logic Here
    this.store.dispatch(new GetUserDetailsAction());
  }

  @Action(UpdatePasswordAction)
  updatePassword(_ctx: StateContext<AuthStateModel>, _action: UpdatePasswordAction) {
    // Update Password Logic Here
  }

  @Action(LogoutAction)
  logout(_ctx: StateContext<AuthStateModel>) {
    this.store.dispatch(new AuthClearAction());
    void this.router.navigate(['/']);
  }

  @Action(AuthClearAction)
  authClear(ctx: StateContext<AuthStateModel>) {
    ctx.patchState({
      email: '',
      token: '',
      access_token: null,
      permissions: [],
    });
    this.authService.redirectUrl = undefined;
    this.store.dispatch(new AccountClearAction());
    this.store.dispatch(new ClearCartAction());
  }
}
