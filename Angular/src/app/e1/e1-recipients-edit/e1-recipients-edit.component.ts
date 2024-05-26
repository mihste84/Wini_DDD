import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { E1RecipientMessage } from '../models/types';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';

@Component({
  selector: 'app-e1-recipients-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormControlComponent],
  templateUrl: './e1-recipients-edit.component.html',
  styleUrl: './e1-recipients-edit.component.css',
})
export class E1RecipientsEditComponent implements OnInit {
  @Input() public recipient?: E1RecipientMessage;
  @Output() public onSaveRecipient = new EventEmitter<E1RecipientMessage>();
  @Output() public onCancel = new EventEmitter<void>();

  public form = new FormGroup({
    recipient: new FormControl('', [Validators.required, Validators.maxLength(200)]),
    message: new FormControl('', [Validators.required, Validators.maxLength(300)]),
  });

  public onSubmitClick() {
    if (this.form.invalid) return;
    this.form.controls.recipient.hasError('required');
    this.onSaveRecipient.emit(this.form.value as E1RecipientMessage);
  }

  ngOnInit(): void {
    if (this.recipient) {
      this.form.setValue(this.recipient);
      this.form.controls.recipient.disable();
    }
  }
}
