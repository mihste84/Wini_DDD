import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1CommentsComponent } from './e1-comments.component';

describe('E1CommentsComponent', () => {
  let component: E1CommentsComponent;
  let fixture: ComponentFixture<E1CommentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1CommentsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1CommentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
