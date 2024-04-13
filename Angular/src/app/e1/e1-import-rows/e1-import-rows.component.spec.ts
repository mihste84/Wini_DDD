import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1ImportRowsComponent } from './e1-import-rows.component';

describe('E1ImportRowsComponent', () => {
  let component: E1ImportRowsComponent;
  let fixture: ComponentFixture<E1ImportRowsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1ImportRowsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1ImportRowsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
