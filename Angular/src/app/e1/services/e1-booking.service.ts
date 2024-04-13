import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BookingValidationResult, E1Booking, E1BookingInput, SqlResult } from '../models/types';

@Injectable({
  providedIn: 'root',
})
export class E1BookingService {
  constructor(private http: HttpClient) {}

  public insertNewBooking(body: E1BookingInput) {
    return this.http.post<SqlResult>('booking', body);
  }

  public updateBooking(bookingId: number, rowVersion: string, body: E1BookingInput) {
    return this.http.patch<SqlResult>('booking/' + bookingId, body, { headers: { rowVersion: rowVersion } });
  }

  public validateBooking(body: E1BookingInput) {
    return this.http.post<BookingValidationResult>('booking/validate', body);
  }

  public validateBookingById(bookingId: number) {
    return this.http.get<BookingValidationResult>(`booking/${bookingId}/validate`);
  }

  public getBookingById(id: number) {
    return this.http.get<E1Booking>('booking/' + id);
  }
}
