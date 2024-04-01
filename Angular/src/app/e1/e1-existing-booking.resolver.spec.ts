import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { e1ExistingBookingResolver } from './e1-existing-booking.resolver';

describe('e1ExistingBookingResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) => 
      TestBed.runInInjectionContext(() => e1ExistingBookingResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
