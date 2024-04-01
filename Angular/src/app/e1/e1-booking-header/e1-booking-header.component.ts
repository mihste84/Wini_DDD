import { Component, OnDestroy, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, NG_VALIDATORS, ControlValueAccessor, FormBuilder, FormGroup, Validators, FormControl, ReactiveFormsModule } from '@angular/forms';
import { Ledger } from '../models/ledger';
import { Subscription } from 'rxjs';
import { E1BookingHeader } from '../models/types';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';

@Component({
  selector: 'app-e1-booking-header',
  standalone: true,
  imports: [ReactiveFormsModule, FormControlComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => E1BookingHeaderComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => E1BookingHeaderComponent),
      multi: true
    }
  ],
  templateUrl: './e1-booking-header.component.html',
  styleUrl: './e1-booking-header.component.css'
})
export class E1BookingHeaderComponent implements ControlValueAccessor, OnDestroy {
  public form: FormGroup;
  private sub?: Subscription;

  get value(): E1BookingHeader {
    return this.form.value;
  }

  set value(value: E1BookingHeader) {
    this.form.setValue(value);
    this.onChange(value);
    this.onTouched();
  }

  constructor(fb: FormBuilder) {
    this.form = fb.group({
      bookingDate: ['', [Validators.required]],
      textToE1: ['', [Validators.maxLength(30)]],
      isReversed: [false],
      ledgerType: [Ledger.AA],
    });

    this.sub = this.form.valueChanges.subscribe(value => {
      this.onChange(value);
      this.onTouched();
    })
  }

  onChange: any = () => {};
  onTouched: any = () => {};

  registerOnChange(fn: any) {
    this.onChange = fn;
  }

  writeValue(value?: E1BookingHeader) {
    if (value) {
      this.value = value;
    }
  }

  registerOnTouched(fn: any) {
    this.onTouched = fn;
  }

  validate(_: FormControl) {
    return this.form.valid ? null : { header: { valid: false } };
  }
  
  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.form.disable() : this.form.enable();
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
