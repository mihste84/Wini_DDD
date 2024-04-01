import { Component, OnDestroy, ViewChild, forwardRef } from '@angular/core';
import {
  ReactiveFormsModule,
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  ControlValueAccessor,
  FormArray,
  FormBuilder,
  FormControl,
  Validators,
  AbstractControl,
  FormGroup,
} from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { NewE1BookingRow } from '../models/types';
import { Subscription } from 'rxjs';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { CommonModule } from '@angular/common';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';

@Component({
  selector: 'app-e1-booking-row-table',
  standalone: true,
  imports: [ReactiveFormsModule, FormControlComponent, FontAwesomeModule, ModalComponent, CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => E1BookingRowTableComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => E1BookingRowTableComponent),
      multi: true,
    },
  ],
  templateUrl: './e1-booking-row-table.component.html',
  styleUrl: './e1-booking-row-table.component.css',
})
export class E1BookingRowTableComponent implements ControlValueAccessor, OnDestroy {
  public form: FormGroup;
  public rows: FormArray;
  public faTrash = faTrash;
  public amountSum = 0;
  public showAllCostObjects = false;

  private sub?: Subscription;
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  get value(): NewE1BookingRow[] {
    return this.rows?.value;
  }

  set value(value: NewE1BookingRow[]) {
    this.form.get('rows')?.setValue(value);
    this.onChange(value);
    this.onTouched();
  }

  constructor(private fb: FormBuilder) {
    this.rows = this.fb.array([]);
    this.form = this.fb.group({
      rows: this.rows,
    });
    this.sub = this.rows.valueChanges.subscribe((value) => {
      this.amountSum = this.getRowSum();
      this.onChange(value);
      this.onTouched();
    });
  }

  public onRemoveRowCallback(index: number) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const rowNumber = index + 1;
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [
        { name: 'message', value: `Are you sure you want to delete row number "${rowNumber}"?` },
        { name: 'returnObject', value: index },
      ],
      `Delete row #${rowNumber}`
    );

    ref?.instance.onConfirm.subscribe((ret: number) => {
      this.rows?.removeAt(ret);
      this.sharedModal!.hideModal();
    });

    ref?.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  public addNewRow() {
    const initValues = this.getLastCurrencyAndBusinessUnit();
    this.rows?.push(this.getFormRow(initValues?.businessUnit, initValues?.currencyCode));
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

  public toggleCostObjects() {
    this.showAllCostObjects = !this.showAllCostObjects;
  }

  private getLastCurrencyAndBusinessUnit() {
    if (!this.rows?.length) return;

    const lastRow = this.rows.value?.slice(-1) as NewE1BookingRow[];
    if (!lastRow?.length) return;

    return { businessUnit: lastRow[0].businessUnit, currencyCode: lastRow[0].currencyCode };
  }


  private getRowSum() {
    const rows = this.rows?.value as NewE1BookingRow[];
    return (
      rows
        ?.filter((_) => _.amount !== undefined)
        .map((_) => _.amount as number)
        .reduce((acc: number, curr: number) => acc + curr, 0) ?? 0
    );
  }

  onChange: any = (value: NewE1BookingRow[]) => {};
  onTouched: any = () => {};

  registerOnChange(fn: any) {
    this.onChange = fn;
  }

  writeValue(value?: NewE1BookingRow[]) {
    if (value) {
      this.value = value;
    }
  }

  registerOnTouched(fn: any) {
    this.onTouched = fn;
  }

  validate(_: FormControl) {
    return this.rows?.valid ? null : { rows: { valid: false } };
  }

  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.rows?.disable() : this.rows?.enable();
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
