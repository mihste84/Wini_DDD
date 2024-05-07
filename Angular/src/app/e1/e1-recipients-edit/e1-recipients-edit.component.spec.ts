import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1RecipientsEditComponent } from './e1-recipients-edit.component';

describe('E1RecipientsEditComponent', () => {
  let component: E1RecipientsEditComponent;
  let fixture: ComponentFixture<E1RecipientsEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1RecipientsEditComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1RecipientsEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
