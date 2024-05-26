import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';

@Component({
  selector: 'app-e1-copy-value',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormControlComponent],
  templateUrl: './e1-copy-value.component.html',
  styleUrl: './e1-copy-value.component.css',
})
export class E1CopyValueComponent implements OnInit {
  @Input({ required: true }) public message!: string;
  @Input({ required: true }) public maxLength!: number;
  @Output() public onConfirm = new EventEmitter<string>();
  @Output() public onCancel = new EventEmitter<void>();

  public form = new FormControl('');

  public onSubmit() {
    if (this.form.invalid) return;

    this.onConfirm.emit(this.form.value?.toString());
  }

  ngOnInit(): void {
    this.form.setValidators([Validators.required, Validators.maxLength(this.maxLength)]);
    this.form.updateValueAndValidity();
  }
}
