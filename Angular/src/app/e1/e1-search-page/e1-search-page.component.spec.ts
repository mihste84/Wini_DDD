import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1SearchPageComponent } from './e1-search-page.component';

describe('E1SearchPageComponent', () => {
  let component: E1SearchPageComponent;
  let fixture: ComponentFixture<E1SearchPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1SearchPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1SearchPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
