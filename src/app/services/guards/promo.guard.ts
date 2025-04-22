import { CanDeactivateFn } from '@angular/router';
import { CanComponentDeactivate } from './register.guard';

export const promoGuard: CanDeactivateFn<CanComponentDeactivate> = (
  component,
  currentRoute,
  currentState,
  nextState
) => {
  return component.canDeactivate()
    ? true
    : confirm(
        "You can't get promo code for another 3 days. Are you sure you have stored your gift?"
      );
};
