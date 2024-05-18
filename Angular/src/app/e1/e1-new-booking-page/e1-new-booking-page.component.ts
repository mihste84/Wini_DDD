import { Component, OnInit, effect } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { CommonModule } from '@angular/common';
import { E1BookingHeaderComponent } from '../e1-booking-header/e1-booking-header.component';
import { firstValueFrom } from 'rxjs';
import { Ledger } from '../models/ledger';
import {
  BookingRowImport,
  BookingValidationResult,
  E1BookingInput,
  E1BookingRow,
  SqlResult,
  E1BookingHeader,
  E1Booking,
} from '../models/types';
import { BannerType, MsgBannerComponent } from '../../shared/components/msg-banner/msg-banner.component';
import { E1ImportRowsComponent } from '../e1-import-rows/e1-import-rows.component';
import { faPlus, faMinus } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoadingService } from '../../shared/services/loading.service';
import { Router } from '@angular/router';
import { NotificationService, NotificationType } from '../../shared/services/notification.service';
import { E1BookingService } from '../services/e1-booking.service';
import { E1BookingRowTableComponent } from '../e1-booking-row-table/e1-booking-row-table.component';
import { getFormattedDateString } from '../../shared/utils/date.utils';

interface E1BookingInputForm {
  header: E1BookingHeader;
  rows?: E1BookingRow[];
}

@Component({
  selector: 'app-e1-new-booking-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormControlComponent,
    ModalComponent,
    CommonModule,
    E1BookingHeaderComponent,
    E1BookingRowTableComponent,
    MsgBannerComponent,
    E1ImportRowsComponent,
    FontAwesomeModule,
  ],
  templateUrl: './e1-new-booking-page.component.html',
  styleUrl: './e1-new-booking-page.component.css',
})
export class E1NewBookingPageComponent {
  public form!: FormGroup;
  public header: FormGroup;
  public rows!: FormArray;
  public validationResult?: BookingValidationResult;
  public types = BannerType;
  public importIcon = faPlus;
  public showImport = false;
  public loading = false;

  constructor(
    private fb: FormBuilder,
    private bookingService: E1BookingService,
    private router: Router,
    private notifications: NotificationService,
    loadingService: LoadingService
  ) {
    const state = this.router.getCurrentNavigation()?.extras.state as { booking: E1Booking };
    const startDate = state?.booking ? new Date(state.booking.bookingDate) : new Date();
    this.header = fb.group({
      bookingDate: [getFormattedDateString(startDate), [Validators.required]],
      textToE1: [state?.booking.textToE1 ?? '', [Validators.maxLength(30)]],
      isReversed: [state?.booking.reversed ?? false],
      ledgerType: [state?.booking.ledgerType ?? Ledger.AA],
    });

    const rows = state?.booking?.rows
      ? state.booking.rows.map((_, i) =>
          E1BookingRowTableComponent.getFormRow({
            rowNumber: i + 1,
            account: _.account,
            amount: _.amount,
            authorizer: _.authorizer,
            businessUnit: _.businessUnit,
            costObject1: _.costObject1,
            costObject2: _.costObject2,
            costObject3: _.costObject3,
            costObject4: _.costObject4,
            costObjectType1: _.costObjectType1,
            costObjectType2: _.costObjectType2,
            costObjectType3: _.costObjectType3,
            costObjectType4: _.costObjectType4,
            currencyCode: _.currencyCode,
            exchangeRate: _.exchangeRate,
            remark: _.remark,
            subledger: _.subledger,
            subledgerType: _.subledgerType,
            subsidiary: _.subsidiary,
            isNew: true,
          })
        )
      : [
          E1BookingRowTableComponent.getFormRow({ rowNumber: 1, isNew: true }),
          E1BookingRowTableComponent.getFormRow({ rowNumber: 2, isNew: true }),
        ];

    this.initForm(rows);

    effect(() => {
      this.loading = loadingService.isLoading();
    });
  }

  public toggeShowImportClick() {
    this.showImport = !this.showImport;
    this.importIcon = this.showImport ? faMinus : faPlus;
  }

  public async validateBookingClick() {
    if (this.form.invalid || this.loading) return;

    const body = this.getBookingFromForm();

    const req = this.bookingService.validateBooking(body);
    this.validationResult = await firstValueFrom(req);
  }

  public onBannerDissmissCallback() {
    this.validationResult = undefined;
  }

  public onImportRowsCallback(rows: BookingRowImport[]) {
    if (!rows?.length) return;

    const newRows = rows.map((row, i) => {
      const mappedRow = E1ImportRowsComponent.mapImportToRow(row, i + 1);
      return E1BookingRowTableComponent.getFormRow(mappedRow);
    });

    this.initForm(newRows);
  }

  public async saveBookingClick() {
    const result = await this.saveBooking();
    this.router.navigate(['e1/edit/' + result?.id]);
  }

  public async saveAndCloseBookingClick() {
    await this.saveBooking();
    this.closeClick();
  }

  public closeClick() {
    this.router.navigate(['e1/search']);
  }

  private async saveBooking() {
    if (this.form.invalid || this.loading) return;

    const body = this.getBookingFromForm();

    const req = this.bookingService.insertNewBooking(body);
    const result = await firstValueFrom(req);
    this.notifications.addNotification(`New booking with ID #${result?.id} created.`, 'Booking created', NotificationType.Info);
    return result;
  }

  private initForm(rows: FormGroup[]) {
    this.rows = this.fb.array(rows);
    this.form = this.fb.group({
      header: this.header,
      rows: this.rows,
    });
  }

  private getBookingFromForm() {
    const value = this.form.value as E1BookingInputForm;
    return {
      ...value.header,
      ledgerType: Number(value.header.ledgerType),
      rows: value.rows,
    } as E1BookingInput;
  }
}
