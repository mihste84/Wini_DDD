import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1BookingHeaderComponent } from './e1-booking-header.component';

describe('E1BookingHeaderComponent', () => {
  let component: E1BookingHeaderComponent;
  let fixture: ComponentFixture<E1BookingHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1BookingHeaderComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1BookingHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
