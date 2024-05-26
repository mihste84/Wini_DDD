import { Component, Input, ViewChild, effect } from '@angular/core';
import { IconDefinition, faL, faMinus, faPlus } from '@fortawesome/free-solid-svg-icons';
import { E1Status, E1RecipientMessage, E1Attachment, E1Comment, E1Booking } from '../models/types';
import { BannerType, MsgBannerComponent } from '../../shared/components/msg-banner/msg-banner.component';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../security/authentication.service';
import { AuthorizationService } from '../../security/authorization.service';
import { LoadingService } from '../../shared/services/loading.service';
import { NotificationService, NotificationType } from '../../shared/services/notification.service';
import { E1AttachmentService } from '../services/e1-attachment.service';
import { E1BookingService } from '../services/e1-booking.service';
import { WiniStatus } from '../models/wini-status';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { firstValueFrom } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { E1StatusHistoryComponent } from '../e1-status-history/e1-status-history.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';
import { E1CommentsComponent } from '../e1-comments/e1-comments.component';
import { E1AttachmentsComponent } from '../e1-attachments/e1-attachments.component';
import { E1RecipientsComponent } from '../e1-recipients/e1-recipients.component';

@Component({
  selector: 'app-e1-booking-view-page',
  standalone: true,
  imports: [
    ModalComponent,
    CommonModule,
    MsgBannerComponent,
    FontAwesomeModule,
    E1StatusHistoryComponent,
    ConfirmComponent,
    E1CommentsComponent,
    E1AttachmentsComponent,
    E1RecipientsComponent,
    ConfirmComponent,
  ],
  templateUrl: './e1-booking-view-page.component.html',
  styleUrl: './e1-booking-view-page.component.css',
})
export class E1BookingViewPageComponent {
  @Input() public booking!: E1Booking;
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public types = BannerType;
  public loading = false;
  public updatedDate!: string;
  public updatedBy!: string;
  public bookingId!: number;
  public rowVersion!: string;
  public statusHistory: E1Status[] = [];
  public recipientMessages: E1RecipientMessage[] = [];
  public attachments: E1Attachment[] = [];
  public comments: E1Comment[] = [];
  public toggleStates: any = {
    history: { value: false, icon: faPlus },
    import: { value: false, icon: faPlus },
    comments: { value: false, icon: faPlus },
    recipients: { value: false, icon: faPlus },
    attachments: { value: false, icon: faPlus },
  };
  public amountSum = 0;
  public showAllCostObjects = false;
  public isAuthorizer = false;
  public isCommissioner = false;

  constructor(
    private router: Router,
    private bookingService: E1BookingService,
    private notifications: NotificationService,
    private attachmentsService: E1AttachmentService,
    private authenticationService: AuthenticationService,
    public authorizationService: AuthorizationService,
    loadingService: LoadingService
  ) {
    effect(() => {
      this.loading = loadingService.isLoading();
    });
  }

  public getStatusName() {
    return WiniStatus[this.booking.status];
  }

  public toggle(value: string) {
    const state = this.toggleStates[value] as { value: boolean; icon: IconDefinition };
    if (!state) return;
    state.value = !state.value;
    state.icon = state.value ? faMinus : faPlus;
  }

  public async uploadAttachmentCallback(files: FileList) {
    if (this.loading) return;

    const req = this.attachmentsService.uploadAttachment(this.bookingId, files);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.notifications.addNotification('Attachments uploaded.', `Attachment uploaded to booking ${this.bookingId}.`, NotificationType.Info);
    const filesToAdd = Array.from(files).map((file) => ({ name: file.name, size: file.size, contentType: file.type, path: '' }));
    this.attachments = [...this.attachments, ...filesToAdd];
  }

  public toggleCostObjectsClick() {
    this.showAllCostObjects = !this.showAllCostObjects;
  }

  public async deleteAttachmentCallback(attachment: E1Attachment) {
    if (this.loading || !this.attachments.length) return;

    const req = this.attachmentsService.deleteComment(this.bookingId, attachment.name);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.notifications.addNotification(
      'Attachments uploaded.',
      `Attachment "${attachment.name}" deleted on booking ${this.bookingId}.`,
      NotificationType.Info
    );
    this.attachments = this.attachments.filter((_) => _ !== attachment);
  }

  public async viewFileCallback(attachment: E1Attachment) {
    if (this.loading) return;
    this.attachmentsService.downloadFile(this.bookingId, attachment.name);
  }

  public closeClick() {
    this.router.navigate(['e1/search']);
  }

  public canReturnBooking() {
    return this.isCommissioner && this.booking.status !== WiniStatus.Sent && this.booking.status !== WiniStatus.Cancelled;
  }

  public canAuthorizeBooking() {
    return this.isAuthorizer && this.authorizationService.isWrite && this.booking.status === WiniStatus.ToBeAuthorized;
  }

  public async returnClick() {
    if (!this.canReturnBooking() || this.loading) return;

    this.confirm(`Are you sure you want to return booking "${this.booking.bookingId}"?`, 'Return booking', () =>
      this.updateBookingStatus(WiniStatus.Saved)
    );
  }

  public copyClick() {
    if (this.loading) return;

    this.confirm('Do you want to create a copy of current booking?', 'Copy booking', async () => {
      this.router.navigate(['e1/new'], { state: { booking: this.booking } });
    });
  }

  public async authorizeClick() {
    if (this.loading || !this.canAuthorizeBooking()) return;

    this.confirm(`Are you sure you want to authorize booking "${this.booking.bookingId}"?`, 'Authorize booking', () =>
      this.updateBookingStatus(WiniStatus.ToBeSent)
    );
  }

  private async confirm(message: string, title: string, onConfirmCb: () => Promise<void>) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');

    const ref = this.sharedModal.showModalWithComponent(ConfirmComponent, [{ name: 'message', value: message }], title);

    ref.instance.onConfirm.subscribe(async () => {
      await onConfirmCb();

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  private async updateBookingStatus(status: WiniStatus) {
    const req = this.bookingService.updateBookingStatus(this.bookingId, this.rowVersion, status);
    await firstValueFrom(req);

    this.notifications.addNotification(
      `Booking with ID #${this.bookingId} updated and status set to ${status}.`,
      'Booking status updated',
      NotificationType.Info
    );
    this.closeClick();
  }

  private init() {
    this.updatedDate = this.booking.updated;
    this.updatedBy = this.booking.updatedBy;
    this.bookingId = this.booking.bookingId;
    this.rowVersion = this.booking.rowVersion;
    this.statusHistory = this.booking.statusHistory;
    this.attachments = this.booking.attachments;
    this.recipientMessages = this.booking.messages;
    this.comments = this.booking.comments;
    const name = this.authenticationService.getAppUser().userName;
    this.isAuthorizer = this.booking.rows.some((_) => _.authorizer === name);
    this.isCommissioner = this.booking.commissioner === name;
  }

  ngOnInit(): void {
    if (!this.booking) {
      this.notifications.addNotification('Could not find booking in database.', 'Booking not found', NotificationType.Error);

      this.closeClick();
    }

    this.init();
    if (this.isAuthorizer && this.isCommissioner) {
      this.notifications.addNotification('You cannot be both authorizer and commissioner.', 'Error', NotificationType.Error);
      this.closeClick();
    }
  }
}
