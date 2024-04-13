import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { E1BookingService } from './services/e1-booking.service';
import { E1Booking } from './models/types';
import { firstValueFrom } from 'rxjs';

export const e1ExistingBookingResolver: ResolveFn<E1Booking> = async (route, _) => {
  const id = route.params['id'];
  const bookingService = inject(E1BookingService);
  const req = bookingService.getBookingById(id);

  return await firstValueFrom(req);
};
