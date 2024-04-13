import { Component, Input, OnInit, ViewChild, effect } from '@angular/core';
import { BookingValidationResult, E1Booking, E1BookingHeader, E1BookingRow, E1Comment, E1Status } from '../models/types';
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
import { AuthService } from '../../security/auth.service';
import { E1StatusHistoryComponent } from '../e1-status-history/e1-status-history.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';
import { E1CommentsComponent } from '../e1-comments/e1-comments.component';
import { E1CommentService } from '../services/e1-comment.service';
import { E1CommentEditComponent } from '../e1-comment-edit/e1-comment-edit.component';
import { getFormattedDateTimeString } from '../../shared/utils/date.utils';

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
  public comments: E1Comment[] = [];
  public toggleStates: any = {
    history: { value: false, icon: faPlus },
    import: { value: false, icon: faPlus },
    comments: { value: false, icon: faPlus },
    recipients: { value: false, icon: faPlus },
  };

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private bookingService: E1BookingService,
    private commentsService: E1CommentService,
    private notifications: NotificationService,
    private authService: AuthService,
    loadingService: LoadingService
  ) {
    effect(() => {
      this.loading = loadingService.isLoading();
    });
  }

  public async reloadClick() {
    if (this.loading) return;

    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [{ name: 'message', value: `Are you sure you want to reload booking? Unsaved changes will be lost.` }],
      'Reload booking #' + this.bookingId
    );

    ref.instance.onConfirm.subscribe(async () => {
      const req = this.bookingService.getBookingById(this.bookingId);
      this.booking = await firstValueFrom(req);
      this.init();

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
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
    const req = this.commentsService.insertNewComment(this.bookingId, this.rowVersion, comment);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.comments.push(comment);
    this.comments = [...this.comments];
  }

  public async commentDeleteCallback(comment: E1Comment) {
    const req = this.commentsService.deleteComment(this.bookingId, this.rowVersion, comment);
    const result = await firstValueFrom(req);
    this.rowVersion = result.rowVersion;

    this.comments = this.comments.filter((_) => _ !== comment);
  }

  public async commentEditCallback(comment: E1Comment) {
    if (this.loading) return;

    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(E1CommentEditComponent, [{ name: 'comment', value: comment }], 'Edit comment');

    ref.instance.onEditComment.subscribe(async (value: string) => {
      const editComment = { ...comment, value };
      const req = this.commentsService.editComment(this.bookingId, this.rowVersion, editComment);
      const result = await firstValueFrom(req);
      this.rowVersion = result.rowVersion;

      editComment.created = getFormattedDateTimeString(new Date()); // Return from backend to show more accurate time
      this.comments = this.comments.map((_) => (_ === comment ? editComment : _));

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  private updateStatuses() {
    this.statusHistory.push({ status: WiniStatus.Saved, updated: this.updatedDate, updatedBy: this.updatedBy });
    this.updatedDate = new Date().toUTCString();
    this.updatedBy = this.authService.getAppUser().userName;
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

    this.init();
  }
}
