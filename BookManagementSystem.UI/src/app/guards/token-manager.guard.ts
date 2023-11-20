import { CanActivateFn } from '@angular/router';

export const tokenManagerGuard: CanActivateFn = (route, state) => {
  return true;
};
