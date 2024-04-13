import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1StatusHistoryComponent } from './e1-status-history.component';

describe('E1StatusHistoryComponent', () => {
  let component: E1StatusHistoryComponent;
  let fixture: ComponentFixture<E1StatusHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1StatusHistoryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1StatusHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
