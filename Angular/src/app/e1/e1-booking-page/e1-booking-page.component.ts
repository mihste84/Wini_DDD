import { Component, Input, OnInit, ViewChild, effect } from '@angular/core';
import {
  BookingRowImport,
  BookingValidationResult,
  E1Attachment,
  E1Booking,
  E1BookingHeader,
  E1BookingRow,
  E1Comment,
  E1RecipientMessage,
  E1Status,
} from '../models/types';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { BannerType, MsgBannerComponent } from '../../shared/components/msg-banner/msg-banner.component';
import { E1BookingHeaderComponent } from '../e1-booking-header/e1-booking-header.component';
import { E1ImportRowsComponent } from '../e1-import-rows/e1-import-rows.component';
import { faPlus, faMinus, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { NotificationService, NotificationType } from '../../shared/services/notification.service';
import { LoadingService } from '../../shared/services/loading.service';
import { WiniStatus } from '../models/wini-status';
import { E1BookingService } from '../services/e1-booking.service';
import { firstValueFrom } from 'rxjs';
import { E1BookingRowTableComponent } from '../e1-booking-row-table/e1-booking-row-table.component';
import { AuthenticationService } from '../../security/authentication.service';
import { E1StatusHistoryComponent } from '../e1-status-history/e1-status-history.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';
import { E1CommentsComponent } from '../e1-comments/e1-comments.component';
import { E1CommentService } from '../services/e1-comment.service';
import { E1CommentEditComponent } from '../e1-comment-edit/e1-comment-edit.component';
import { getFormattedDateTimeString } from '../../shared/utils/date.utils';
import { E1AttachmentsComponent } from '../e1-attachments/e1-attachments.component';
import { E1AttachmentService } from '../services/e1-attachment.service';
import { E1RecipientsComponent } from '../e1-recipients/e1-recipients.component';
import { E1RecipientMessageService } from '../services/e1-recipient-message.service';
import { AuthorizationService } from '../../security/authorization.service';

interface E1BookingForm {
  header: E1BookingHeader;
  rows?: E1BookingRow[];
  bookingId: number;
  rowVersion: string;
}

@Component({
  selector: 'app-e1-booking-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormControlComponent,
    ModalComponent,
    CommonModule,
    E1BookingHeaderComponent,
    E1BookingRowTableComponent,
    HttpClientModule,
    MsgBannerComponent,
    E1ImportRowsComponent,
    FontAwesomeModule,
    E1StatusHistoryComponent,
    ConfirmComponent,
    E1CommentsComponent,
    E1CommentEditComponent,
    E1AttachmentsComponent,
    E1RecipientsComponent,
  ],
  templateUrl: './e1-booking-page.component.html',
  styleUrl: './e1-booking-page.component.css',
})
export class E1BookingPageComponent implements OnInit {
  @Input() public booking!: E1Booking;
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public form!: FormGroup;
  public header!: FormGroup;
  public rows!: FormArray;
  public validationResult?: BookingValidationResult;
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

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private bookingService: E1BookingService,
    private commentsService: E1CommentService,
    private notifications: NotificationService,
    private attachmentsService: E1AttachmentService,
    private recipientService: E1RecipientMessageService,
    private authenticationService: AuthenticationService,
    private authorizationService: AuthorizationService,
    loadingService: LoadingService
  ) {
    effect(() => {
      this.loading = loadingService.isLoading();
    });
  }

  public async validateBookingClick() {
    if (this.form.invalid || this.loading) return;
    if (this.form.dirty) await this.saveBookingClick();

    const id = this.bookingId;

    const req = this.bookingService.validateBookingById(id);
    this.validationResult = await firstValueFrom(req);
  }

  public getStatusName() {
    return WiniStatus[this.booking.status];
  }

  public onBannerDissmissCallback() {
    this.validationResult = undefined;
  }

  public toggle(value: string) {
    const state = this.toggleStates[value] as { value: boolean; icon: IconDefinition };
    if (!state) return;
    state.value = !state.value;
    state.icon = state.value ? faMinus : faPlus;
  }

  public async saveBookingClick() {
    if (this.form.invalid || this.loading) return;

    await this.saveBooking();

    this.updateFormAfterSave();
    this.updateStatuses();
  }

  public async commentCreateCallback(comment: E1Comment) {
    if (this.loading) return;

    const req = this.commentsService.insertNewComment(this.bookingId, comment);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.comments.push(comment);
    this.comments = [...this.comments];
  }

  public async commentDeleteCallback(comment: E1Comment) {
    if (this.loading || !this.comments.length) return;

    const req = this.commentsService.deleteComment(this.bookingId, comment);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.comments = this.comments.filter((_) => _ !== comment);
  }

  public async commentEditCallback(comment: { original: E1Comment; newValue: string }) {
    if (this.loading || !this.comments.length) return;

    const editedComment = { created: comment.original.created, value: comment.newValue };
    const req = this.commentsService.editComment(this.bookingId, editedComment);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    editedComment.created = getFormattedDateTimeString(new Date());
    this.comments = this.comments.map((_) => (_ === comment.original ? editedComment : _));
  }

  public importRowsCallback(rows: BookingRowImport[]) {
    if (!rows?.length || this.loading) return;

    const maxRowNumber = (this.form.get('maxRowNumber')?.value as number) ?? 0;

    rows.forEach((row) => {
      const rowNumber = E1BookingRowTableComponent.getNextRowNumberForNewRow(this.rows.value, maxRowNumber);
      const e1Row = E1ImportRowsComponent.mapImportToRow(row, rowNumber);
      this.rows.push(E1BookingRowTableComponent.getFormRow(e1Row));
    });
    this.form.markAsDirty();
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

  public async recipientMessageEditCallback(evt: { original: E1RecipientMessage; newValue: E1RecipientMessage }) {
    if (this.loading || !this.recipientMessages.length) return;

    const newRecipient = {
      recipient: evt.original.recipient,
      message: evt.newValue.message,
    };
    const req = this.recipientService.editRecipientMessage(this.bookingId, newRecipient);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.recipientMessages = this.recipientMessages.map((_) => (_ === evt.original ? newRecipient : _));
  }

  public async recipientMessageDeletedCallback(evt: E1RecipientMessage) {
    if (this.loading || !this.recipientMessages.length) return;

    const req = this.recipientService.deleteRecipientMessage(this.bookingId, evt);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.recipientMessages = this.recipientMessages.filter((_) => _ !== evt);
  }

  public async recipientMessageCreatedCallback(evt: E1RecipientMessage) {
    if (this.loading) return;
    if (this.recipientMessages.length == 5) {
      this.notifications.addNotification('Maximum number of messages reached.', 'Maximum messages reached', NotificationType.Warning);
      return;
    }

    const req = this.recipientService.insertNewRecipientMessage(this.bookingId, evt);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.recipientMessages.push(evt);
    this.recipientMessages = [...this.recipientMessages];
  }

  public async viewFileCallback(attachment: E1Attachment) {
    if (this.loading) return;
    this.attachmentsService.downloadFile(this.bookingId, attachment.name);
  }

  public async saveAndSendClick() {
    if (this.form.invalid || this.loading) return;

    await this.validateBookingClick();

    if (!this.validationResult?.isValid) return;

    const statusToChange = this.authorizationService.isBookingAuthorizationNeeded ? WiniStatus.ToBeAuthorized : WiniStatus.ToBeSent;
    const req = this.bookingService.updateBookingStatus(this.bookingId, this.rowVersion, statusToChange);
    await firstValueFrom(req);

    const message = `Booking with ID #${this.bookingId} updated and status set to ${WiniStatus[statusToChange]}.`;
    this.notifications.addNotification(message, 'Booking status updated', NotificationType.Info);
    this.router.navigate(['e1/search']);
  }

  private updateStatuses() {
    this.statusHistory.push({ status: WiniStatus.Saved, updated: this.updatedDate, updatedBy: this.updatedBy });
    this.updatedDate = new Date().toUTCString();
    this.updatedBy = this.authenticationService.getAppUser().userName;
  }

  private updateFormAfterSave() {
    const rowValues = this.rows.value as E1BookingRow[];
    const maxRowNumber = Math.max(...rowValues.map((_) => _.rowNumber));
    const rows = rowValues
      .filter((_) => !_.toDelete)
      .map((_) => {
        _.isNew = false;
        return E1BookingRowTableComponent.getFormRow(_);
      });
    this.initForm(maxRowNumber, rows);
    this.form.markAsPristine();
  }

  private async saveBooking() {
    const body = this.getBookingFromForm();
    const req = this.bookingService.updateBooking(this.bookingId, this.rowVersion, body);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.notifications.addNotification(`Booking with ID #${result?.id} updated.`, 'Booking updated', NotificationType.Info);
  }

  private initForm(maxRowNumber: number, rows: FormGroup[]) {
    this.rows = this.fb.array(rows);
    this.form = this.fb.group({
      header: this.header,
      rows: this.rows,
      maxRowNumber: this.fb.control(maxRowNumber),
    });
  }

  private init() {
    this.header = this.fb.group({
      bookingDate: [this.booking.bookingDate, [Validators.required]],
      textToE1: [this.booking.textToE1, [Validators.maxLength(30)]],
      isReversed: [this.booking.reversed],
      ledgerType: [this.booking.ledgerType],
    });
    this.updatedDate = this.booking.updated;
    this.updatedBy = this.booking.updatedBy;
    this.bookingId = this.booking.bookingId;
    this.rowVersion = this.booking.rowVersion;
    this.statusHistory = this.booking.statusHistory;
    this.attachments = this.booking.attachments;
    this.recipientMessages = this.booking.messages;
    this.comments = this.booking.comments;
    const rows = this.booking.rows.map((row) => E1BookingRowTableComponent.getFormRow(row));
    this.initForm(this.booking.maxRowNumber, rows);
  }

  private getBookingFromForm() {
    const value = this.form.value as E1BookingForm;
    return {
      ...value.header,
      ledgerType: Number(value.header.ledgerType),
      rows: value.rows?.filter((_) => !_.toDelete),
      rowNumbersToDelete: value.rows?.filter((_) => !_.isNew && _.toDelete).map((_) => _.rowNumber) ?? [],
    };
  }

  ngOnInit(): void {
    if (!this.booking) {
      this.router.navigate(['e1/search']);
    }

    if (this.booking.status != WiniStatus.Saved) {
      this.notifications.addNotification(
        'Booking can only be edited when status is "Saved".',
        'Booking not in valid state',
        NotificationType.Warning
      );
      this.router.navigate(['e1/search']);
    }

    this.init();
  }
}
