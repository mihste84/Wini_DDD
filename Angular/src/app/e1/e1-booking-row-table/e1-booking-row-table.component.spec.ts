import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1BookingRowTableComponent } from './e1-booking-row-table.component';

describe('E1BookingRowTableComponent', () => {
  let component: E1BookingRowTableComponent;
  let fixture: ComponentFixture<E1BookingRowTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1BookingRowTableComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(E1BookingRowTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
