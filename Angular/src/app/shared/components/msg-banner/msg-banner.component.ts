import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';

export enum BannerType {
  info,
  warning,
  error,
}

@Component({
  selector: 'app-msg-banner',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule],
  templateUrl: './msg-banner.component.html',
  styleUrl: './msg-banner.component.css',
})
export class MsgBannerComponent {
  @Input() public type: BannerType = BannerType.info;
  @Output() public onDismiss = new EventEmitter<void>();

  public types = BannerType;
  public faXmark = faXmark;
}
