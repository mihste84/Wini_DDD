import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MsgBannerComponent } from './msg-banner.component';

describe('MsgBannerComponent', () => {
  let component: MsgBannerComponent;
  let fixture: ComponentFixture<MsgBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MsgBannerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MsgBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
