import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { CommonModule } from '@angular/common';
import { E1BookingHeaderComponent } from '../e1-booking-header/e1-booking-header.component';
import { E1BookingRowTableComponent } from '../e1-booking-row-table/e1-booking-row-table.component';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Ledger } from '../models/ledger';
import { NewE1BookingRow } from '../models/types';

interface NewE1BookingForm {
  header: {
    bookingDate?: string;
    textToE1?: string;
    isReversed: boolean;
    ledgerType: Ledger;
  },
  rows?: NewE1BookingRow[]
}

@Component({
  selector: 'app-e1-new-booking-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormControlComponent,
    FontAwesomeModule,
    ModalComponent,
    CommonModule,
    E1BookingHeaderComponent,
    E1BookingRowTableComponent,
    HttpClientModule
  ],
  templateUrl: './e1-new-booking-page.component.html',
  styleUrl: './e1-new-booking-page.component.css',
})
export class E1NewBookingPageComponent {
  public form: FormGroup;
  public faTrash = faTrash;
  public amountSum = 0;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.form = fb.group({
      header: undefined,
      rows: this.fb.array([
        this.getFormRow()
      ]),
    });
  }

  public getFormRow(businessUnit?: string, currencyCode?: string) {
    return this.fb.group({
      businessUnit: [businessUnit, Validators.maxLength(12)],
      account: ['', Validators.maxLength(6)],
      subsidiary: ['', Validators.maxLength(8)],
      subledger: ['', Validators.maxLength(8)],
      subledgerType: ['', Validators.maxLength(1)],
      costObject1: ['', Validators.maxLength(12)],
      costObjectType1: ['', Validators.maxLength(1)],
      costObject2: ['', Validators.maxLength(12)],
      costObjectType2: ['', Validators.maxLength(1)],
      costObject3: ['', Validators.maxLength(12)],
      costObjectType3: ['', Validators.maxLength(1)],
      costObject4: ['', Validators.maxLength(12)],
      costObjectType4: ['', Validators.maxLength(1)],
      remark: ['', Validators.maxLength(30)],
      authorizer: ['', Validators.maxLength(200)],
      amount: [0],
      currencyCode: [currencyCode, Validators.maxLength(3)],
      exchangeRate: [0, Validators.min(0)],
    });
  }

  public async validateBooking() {
    if (this.form.invalid) return;
    const value = this.form.value as NewE1BookingForm;
    const body = {...value.header, rows: value.rows };
    console.log(body)
    //const req = this.http.post('booking/validate', body);
    //const res = await firstValueFrom(req);
    //console.log(res);
  }
}
