import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1RecipientsComponent } from './e1-recipients.component';

describe('E1RecipientsComponent', () => {
  let component: E1RecipientsComponent;
  let fixture: ComponentFixture<E1RecipientsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1RecipientsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1RecipientsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
