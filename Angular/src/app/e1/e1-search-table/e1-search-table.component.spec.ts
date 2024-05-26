import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1SearchTableComponent } from './e1-search-table.component';

describe('E1SearchTableComponent', () => {
  let component: E1SearchTableComponent;
  let fixture: ComponentFixture<E1SearchTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1SearchTableComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1SearchTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
