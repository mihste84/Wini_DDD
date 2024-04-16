import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1AttachmentsComponent } from './e1-attachments.component';

describe('E1AttachmentsComponent', () => {
  let component: E1AttachmentsComponent;
  let fixture: ComponentFixture<E1AttachmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1AttachmentsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1AttachmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
