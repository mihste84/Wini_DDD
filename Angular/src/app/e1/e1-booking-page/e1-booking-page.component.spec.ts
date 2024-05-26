import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1BookingPageComponent } from './e1-booking-page.component';

describe('E1BookingPageComponent', () => {
  let component: E1BookingPageComponent;
  let fixture: ComponentFixture<E1BookingPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1BookingPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1BookingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
