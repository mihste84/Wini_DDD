import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ReactiveFormsModule, FormArray, FormGroup, FormControl, Validators } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { BookingValidationError, E1BookingRow } from '../models/types';
import { Subscription } from 'rxjs';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { CommonModule } from '@angular/common';
import { FormatAmountDirective } from '../../shared/directives/format-amount.directive';
import { NgxPopperjsModule, NgxPopperjsPlacements, NgxPopperjsTriggers } from 'ngx-popperjs';
import { E1CopyValueComponent } from '../e1-copy-value/e1-copy-value.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';
import { v4 as uuidv4 } from 'uuid';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-e1-booking-row-table',
  standalone: true,
  imports: [
    NgxPopperjsModule,
    ReactiveFormsModule,
    FormControlComponent,
    FontAwesomeModule,
    ModalComponent,
    CommonModule,
    FormatAmountDirective,
    E1CopyValueComponent,
  ],
  providers: [],
  templateUrl: './e1-booking-row-table.component.html',
  styleUrl: './e1-booking-row-table.component.css',
})
export class E1BookingRowTableComponent implements OnDestroy, OnInit {
  @Input() public rows!: FormArray;
  @Input() public form!: FormGroup;
  @Input() public isNewBooking = true;
  @Input() public validationErrors?: BookingValidationError[];

  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public popperplacement = NgxPopperjsPlacements.RIGHTEND;
  public popperTrigger = NgxPopperjsTriggers.click;
  public faTrash = faTrash;
  public amountSum = 0;
  public showAllCostObjects = false;
  public isDevelopment = environment.env === 'local';
  public selectAllToDelete = new FormControl(false);

  private subs: Subscription[] = [];

  public getValidationErrors(index: number) {
    if (!this.validationErrors) return [];
    return this.validationErrors.filter((_) => _.propertyName.replace('#', '') === index.toString()).map((_) => _.message);
  }

  public isValidationError(index: number) {
    if (!this.validationErrors) return false;
    return this.validationErrors.some((_) => _.propertyName.replace('#', '') === index.toString());
  }

  public toggleCostObjects() {
    this.showAllCostObjects = !this.showAllCostObjects;
  }

  public onRemoveRowClick(rowNumber: number, index: number) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [{ name: 'message', value: `Are you sure you want to delete row number "${index}"?` }],
      `Delete row #${index}`
    );

    ref?.instance.onConfirm.subscribe(() => {
      this.rows?.removeAt(index - 1);
      this.reorderNewRows();

      this.sharedModal!.hideModal();
    });

    ref?.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  public onCopyCallback(name: string, maxLength: number, controlName: string) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      E1CopyValueComponent,
      [
        { name: 'message', value: `Copy ${name} to all rows?` },
        { name: 'maxLength', value: maxLength },
      ],
      `Copy ${name}`
    );

    ref?.instance.onConfirm.subscribe((ret: string) => {
      const controlsToSet =
        controlName === 'authorizer' ? this.rows?.controls.filter((control) => control.get('amount')?.value > 0) : this.rows?.controls;

      controlsToSet?.forEach((control) => {
        control.get(controlName)?.setValue(ret);
      });

      this.sharedModal!.hideModal();
    });

    ref?.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  public onAddNewRowClick() {
    const initValues = this.getLastCurrencyAndBusinessUnit();
    this.rows?.push(E1BookingRowTableComponent.getFormRow(initValues));
    this.form.markAsDirty();
  }

  public static getFormRow(row: E1BookingRow | { isNew: boolean; rowNumber: number; businessUnit?: string; currencyCode?: string }) {
    const bookingRow = row as E1BookingRow;
    return new FormGroup({
      id: new FormControl(uuidv4()),
      rowNumber: new FormControl(bookingRow?.rowNumber),
      businessUnit: new FormControl(bookingRow?.businessUnit ?? '', Validators.maxLength(12)),
      account: new FormControl(bookingRow?.account ?? '', Validators.maxLength(6)),
      subsidiary: new FormControl(bookingRow?.subsidiary ?? '', Validators.maxLength(8)),
      subledger: new FormControl(bookingRow?.subledger ?? '', Validators.maxLength(8)),
      subledgerType: new FormControl(bookingRow?.subledgerType ?? '', Validators.maxLength(1)),
      costObject1: new FormControl(bookingRow?.costObject1 ?? '', Validators.maxLength(12)),
      costObjectType1: new FormControl(bookingRow?.costObjectType1 ?? '', Validators.maxLength(1)),
      costObject2: new FormControl(bookingRow?.costObject2 ?? '', Validators.maxLength(12)),
      costObjectType2: new FormControl(bookingRow?.costObjectType2 ?? '', Validators.maxLength(1)),
      costObject3: new FormControl(bookingRow?.costObject3 ?? '', Validators.maxLength(12)),
      costObjectType3: new FormControl(bookingRow?.costObjectType3 ?? '', Validators.maxLength(1)),
      costObject4: new FormControl(bookingRow?.costObject4 ?? '', Validators.maxLength(12)),
      costObjectType4: new FormControl(bookingRow?.costObjectType4 ?? '', Validators.maxLength(1)),
      remark: new FormControl(bookingRow?.remark ?? '', Validators.maxLength(30)),
      authorizer: new FormControl(bookingRow?.authorizer ?? '', Validators.maxLength(200)),
      amount: new FormControl(Number(bookingRow?.amount ?? 0.0).toFixed(2)),
      currencyCode: new FormControl(row?.currencyCode ?? '', Validators.maxLength(3)),
      exchangeRate: new FormControl(Number(bookingRow?.exchangeRate ?? 0.0).toFixed(7), Validators.min(0)),
      isNew: new FormControl(bookingRow?.isNew ?? false),
      toDelete: new FormControl(false),
    });
  }

  private reorderNewRows() {
    let maxRowNumber = this.form.get('maxRowNumber')?.value ?? 0;

    this.rows?.controls
      .filter((_) => _.get('isNew')?.value)
      .forEach((control) => {
        maxRowNumber = maxRowNumber + 1;
        control.get('rowNumber')?.setValue(maxRowNumber);
      });
  }

  private getNextRowNumberForNewRow() {
    let maxRowNumber = (this.form.get('maxRowNumber')?.value as number) ?? 0;

    this.rows?.controls
      .filter((_) => _.get('isNew')?.value)
      .forEach(() => {
        maxRowNumber = maxRowNumber + 1;
      });
    return maxRowNumber + 1;
  }

  private getLastCurrencyAndBusinessUnit() {
    const rowNumber = this.getNextRowNumberForNewRow();
    if (!this.rows?.length) return { isNew: true, rowNumber: rowNumber };

    const lastRow = this.rows.value?.slice(-1) as E1BookingRow[];
    if (!lastRow?.length) return { isNew: true, rowNumber: rowNumber };

    return { isNew: true, businessUnit: lastRow[0].businessUnit, currencyCode: lastRow[0].currencyCode, rowNumber: rowNumber };
  }

  private getRowSum() {
    const rows = this.rows?.value as E1BookingRow[];
    return (
      rows
        ?.filter((_) => _.amount !== undefined)
        .map((_) => _.amount!)
        .reduce((acc: number, curr: any) => acc + Number(curr), 0) ?? 0
    );
  }

  ngOnDestroy(): void {
    this.subs?.forEach((_) => _.unsubscribe());
  }

  ngOnInit(): void {
    this.subs.push(
      this.rows.valueChanges.subscribe(() => {
        this.amountSum = this.getRowSum();
      })
    );

    if (!this.isNewBooking) {
      this.subs.push(
        this.selectAllToDelete.valueChanges.subscribe((value) => {
          this.rows?.controls.forEach((control) => {
            control.get('toDelete')?.setValue(value);
          });
          this.form.markAsDirty();
        })
      );
    }
  }
}
