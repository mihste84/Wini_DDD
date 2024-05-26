import { Directive, HostListener, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: '[appFormatAmount]',
  standalone: true,
})
export class FormatAmountDirective {
  @Input() decimalNo = 2;
  @Input() addSeparator = true;
  constructor(@Optional() @Self() private control: NgControl) {
    if (!this.control) {
      console.error('appFormatAmount can only be used with a formControl');
    }
  }

  @HostListener('focus')
  onFocus() {
    let value = Number(this.control.value);

    this.control.control?.setValue(value === 0 || isNaN(value) ? '' : value);
  }

  @HostListener('blur')
  onBlur() {
    let value = Number(this.control.value);
    if (isNaN(value)) {
      value = 0.0;
    }

    this.control.control?.setValue(this.appendDecimal(value));
  }

  private hasDecimal(value: number): boolean {
    return value?.toString().includes('.');
  }

  private appendDecimal(value: number) {
    return this.hasDecimal(value) ? value : value.toFixed(this.decimalNo);
  }
}
