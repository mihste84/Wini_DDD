import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1BookingViewPageComponent } from './e1-booking-view-page.component';

describe('E1BookingViewPageComponent', () => {
  let component: E1BookingViewPageComponent;
  let fixture: ComponentFixture<E1BookingViewPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1BookingViewPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1BookingViewPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
