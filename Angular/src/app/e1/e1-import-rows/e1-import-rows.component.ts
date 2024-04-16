import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { BookingRowImport, E1BookingRow } from '../models/types';
import csv from 'csvtojson';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faQuestionCircle } from '@fortawesome/free-regular-svg-icons';
import { NgxPopperjsModule, NgxPopperjsPlacements, NgxPopperjsTriggers } from 'ngx-popperjs';

@Component({
  selector: 'app-e1-import-rows',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormControlComponent, FontAwesomeModule, NgxPopperjsModule],
  templateUrl: './e1-import-rows.component.html',
  styleUrl: './e1-import-rows.component.css',
})
export class E1ImportRowsComponent {
  @Output() public onRowsImported = new EventEmitter<BookingRowImport[]>();
  public fileImport = new FormControl();
  public pastedImport = new FormControl();
  public faQuestionCircle = faQuestionCircle;
  public popperplacement = NgxPopperjsPlacements.RIGHTEND;
  public popperTrigger = NgxPopperjsTriggers.hover;

  public importFromFile(input: any) {
    if (!input.files) return;
    const reader = new FileReader();
    reader.onload = (e) => {
      const target = e.currentTarget as FileReader;
      const result = target.result as string;

      if (!result) return;

      csv({
        delimiter: [';', ',', '\t'],
        trim: true,
      })
        .fromString(result)
        .then(
          (_: any[]) => {
            const rows = _.map((row) => {
              return {
                account: row['Account'],
                accountingDate: row['Accounting date'],
                authorizer: row['Authorizer'],
                bookingGroup: row['BookingGroup'],
                businessUnit: row['Business unit'],
                costObject: row['Cost object'],
                costObjectType: row['Cost object type'],
                costObject2: row['Cost object 2'],
                costObjectType2: row['Cost object 2 type'],
                costObject3: row['Cost object 3'],
                costObjectType3: row['Cost object 3 type'],
                costObject4: row['Cost object 4'],
                costObjectType4: row['Cost object 4 type'],
                credit: row['Credit'],
                currency: row['Currency'],
                debit: row['Debit'],
                exchangeRate: row['Exchange rate'],
                glre: row['GLRE'],
                remark: row['Remark'],
                subledger: row['Subledger'],
                subledgerType: row['Subledger type'],
                subsidiary: row['Subsidiary'],
                textToE1: row['Text to E1'],
                useGPLedger: row['Use GP ledger'],
              };
            });

            this.importRows(rows);
          },
          (err) => console.log(err)
        );
    };
    reader.readAsText(input.files[0]);
  }

  public importFromPastedText() {
    const value = this.pastedImport.value;
    if (!value) return;
    csv({
      delimiter: [';', ',', '\t'],
      trim: true,
      noheader: true,
    })
      .fromString(value)
      .then(
        (_: any[]) => {
          const rows = _.map((row) => {
            return {
              businessUnit: row.field1,
              account: row.field2,
              subsidiary: row.field3,
              subledger: row.field4,
              subledgerType: row.field5,
              costObject: row.field6,
              costObjectType: row.field7,
              debit: row.field8,
              credit: row.field9,
              remark: row.field10,
              currency: row.field11,
              exchangeRate: row.field12,
              authorizer: row.field13,
              bookingGroup: row.field14,
              accountingDate: row.field15,
              textToE1: row.field16,
              useGPLedger: row.field19,
              costObject2: row.field20,
              costObjectType2: row.field21,
              costObject3: row.field22,
              costObjectType3: row.field23,
              costObject4: row.field24,
              costObjectType4: row.field25,
              glre: row.field26,
            };
          });

          this.importRows(rows);
        },
        (err) => console.log(err)
      );
  }

  public importRows(rows: BookingRowImport[]) {
    if (!rows?.length) return;

    this.onRowsImported.emit(rows);
    this.resetControls();
  }

  public static mapImportToRow(imp: BookingRowImport, index: number): E1BookingRow {
    const debit = Number(imp.debit?.replace(',', '.'));
    const credit = Number(imp.credit?.replace(',', '.'));
    const amount = !isNaN(debit) && debit > 0 ? debit : !isNaN(credit) && credit > 0 ? credit * -1 : credit;
    const exchangeRate = Number(imp.exchangeRate?.replace(',', '.'));

    return {
      rowNumber: index,
      account: imp.account,
      amount: isNaN(amount) ? 0 : amount,
      authorizer: imp.authorizer,
      businessUnit: imp.businessUnit,
      costObject1: imp.costObject,
      costObject2: imp.costObject2,
      costObject3: imp.costObject3,
      costObject4: imp.costObject4,
      costObjectType1: imp.costObjectType,
      costObjectType2: imp.costObjectType2,
      costObjectType3: imp.costObjectType3,
      costObjectType4: imp.costObjectType4,
      currencyCode: imp.currency,
      exchangeRate: isNaN(exchangeRate) ? 0 : exchangeRate,
      remark: imp.remark,
      subledger: imp.subledger,
      subledgerType: imp.subledgerType,
      subsidiary: imp.subsidiary,
      isNew: true,
    };
  }

  private resetControls() {
    this.fileImport.reset();
    this.pastedImport.reset();
  }
}
