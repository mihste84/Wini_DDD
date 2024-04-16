import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { E1RecipientMessage } from '../models/types';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { CommonModule } from '@angular/common';
import { faPencil, faTrash } from '@fortawesome/free-solid-svg-icons';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-e1-recipients',
  standalone: true,
  imports: [ModalComponent, CommonModule],
  templateUrl: './e1-recipients.component.html',
  styleUrl: './e1-recipients.component.css',
})
export class E1RecipientsComponent {
  @Input({ required: true }) public recipients: E1RecipientMessage[] = [];
  @Input({ required: true }) public loading: boolean = false;
  @Output() public onRecipientMessageCreated = new EventEmitter<E1RecipientMessage>();
  @Output() public onRecipientMessageDeleted = new EventEmitter<E1RecipientMessage>();
  @Output() public onRecipientMessageEdit = new EventEmitter<{ original: E1RecipientMessage; newValue: E1RecipientMessage }>();
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public form = new FormArray([
    new FormGroup({
      recipient: new FormControl('', [Validators.required, Validators.maxLength(200)]),
      message: new FormControl('', [Validators.required, Validators.maxLength(300)]),
    }),
  ]);
  public faTrash = faTrash;
  public faPencil = faPencil;
}
