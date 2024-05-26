import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1CopyValueComponent } from './e1-copy-value.component';

describe('E1CopyValueComponent', () => {
  let component: E1CopyValueComponent;
  let fixture: ComponentFixture<E1CopyValueComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1CopyValueComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1CopyValueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
