import { TestBed } from '@angular/core/testing';
import { CanDeactivateFn } from '@angular/router';

import { registerGuard } from './register.guard';

describe('registerGuard', () => {
  const executeGuard: CanDeactivateFn<unknown> = (...guardParameters) => 
      TestBed.runInInjectionContext(() => registerGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
