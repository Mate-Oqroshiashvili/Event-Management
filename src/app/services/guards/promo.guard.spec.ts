import { TestBed } from '@angular/core/testing';
import { CanDeactivateFn } from '@angular/router';

import { promoGuard } from './promo.guard';

describe('promoGuard', () => {
  const executeGuard: CanDeactivateFn<unknown> = (...guardParameters) => 
      TestBed.runInInjectionContext(() => promoGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
