import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AppNotification, NotificationType } from '../../services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification-card',
  templateUrl: './notification-card.component.html',
  styleUrls: ['./notification-card.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class NotificationCardComponent {
  @Input() public notification?: AppNotification;
  @Output() public onRemove: EventEmitter<AppNotification> = new EventEmitter<AppNotification>();

  public removeNotification() {
    this.onRemove.emit(this.notification);
  }

  public getClassByType(type?: NotificationType) {
    switch (type) {
      case NotificationType.Info:
        return 'border-blue-500';
      case NotificationType.Warning:
        return 'border-yellow-600';
      case NotificationType.Error:
        return 'border-red-600';
      case NotificationType.Success:
        return 'border-green-600';
      default:
        return '';
    }
  }
}
