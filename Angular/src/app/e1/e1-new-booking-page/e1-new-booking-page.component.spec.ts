import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1NewBookingPageComponent } from './e1-new-booking-page.component';

describe('E1NewBookingPageComponent', () => {
  let component: E1NewBookingPageComponent;
  let fixture: ComponentFixture<E1NewBookingPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1NewBookingPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1NewBookingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
