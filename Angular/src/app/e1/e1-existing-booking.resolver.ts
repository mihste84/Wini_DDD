import { ResolveFn } from '@angular/router';

export const e1ExistingBookingResolver: ResolveFn<boolean> = (route, _) => {
  const id = route.params['id'];
  console.log(id)
  return true;
};
