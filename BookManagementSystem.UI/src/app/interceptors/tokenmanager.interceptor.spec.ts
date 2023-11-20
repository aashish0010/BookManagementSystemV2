import { TestBed } from '@angular/core/testing';

import { TokenmanagerInterceptor } from './tokenmanager.interceptor';

describe('TokenmanagerInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      TokenmanagerInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: TokenmanagerInterceptor = TestBed.inject(TokenmanagerInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
