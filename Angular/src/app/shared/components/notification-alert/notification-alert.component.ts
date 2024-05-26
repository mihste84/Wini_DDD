import { Component, Input, effect } from '@angular/core';
import { AppNotification, NotificationService } from '../../services/notification.service';
import { NotificationCardComponent } from '../notification-card/notification-card.component';

@Component({
  selector: 'app-notification-alert',
  templateUrl: './notification-alert.component.html',
  styleUrls: ['./notification-alert.component.css'],
  standalone: true,
  imports: [NotificationCardComponent]
})
export class NotificationAlertComponent {
  @Input() public showDuration: number = 8000;
  @Input() public maxNotifications: number = 5;

  public activeNotifications: AppNotification[] = [];

  constructor(private notificationService: NotificationService) {
    effect(() => {
      const latest = this.notificationService.latestNotificationSig();
      this.addNotificationWithTimeout(latest);
    });
  }

  public removeNotificationCallback(value: AppNotification) {
    this.activeNotifications = this.activeNotifications.filter((_) => _.id !== value.id);
  }

  private addNotificationWithTimeout(notification: AppNotification | undefined) {
    if (!notification) return;

    if (this.activeNotifications.length >= this.maxNotifications) this.activeNotifications.pop();
    this.activeNotifications.push(notification);

    if (this.showDuration === 0) return;
    setTimeout(() => {
      this.removeNotificationCallback(notification);
    }, this.showDuration);
  }
}
