import { ComponentFixture, TestBed } from '@angular/core/testing';

import { E1CommentEditComponent } from './e1-comment-edit.component';

describe('E1CommentEditComponent', () => {
  let component: E1CommentEditComponent;
  let fixture: ComponentFixture<E1CommentEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [E1CommentEditComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(E1CommentEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
