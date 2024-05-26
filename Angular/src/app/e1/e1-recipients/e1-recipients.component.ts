import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { E1RecipientMessage } from '../models/types';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { CommonModule } from '@angular/common';
import { E1RecipientsEditComponent } from '../e1-recipients-edit/e1-recipients-edit.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
@Component({
  selector: 'app-e1-recipients',
  standalone: true,
  imports: [ModalComponent, CommonModule, E1RecipientsEditComponent, ConfirmComponent, FontAwesomeModule],
  templateUrl: './e1-recipients.component.html',
  styleUrl: './e1-recipients.component.css',
})
export class E1RecipientsComponent {
  @Input({ required: true }) public recipients: E1RecipientMessage[] = [];
  @Input({ required: true }) public loading: boolean = false;
  @Input() public readonly: boolean = false;
  @Output() public onRecipientMessageCreated = new EventEmitter<E1RecipientMessage>();
  @Output() public onRecipientMessageDeleted = new EventEmitter<E1RecipientMessage>();
  @Output() public onRecipientMessageEdit = new EventEmitter<{ original: E1RecipientMessage; newValue: E1RecipientMessage }>();
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public faTimes = faTimes;

  public onAddOrEditRecipientClick(recipient?: E1RecipientMessage) {
    if (this.loading || this.readonly) return;

    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      E1RecipientsEditComponent,
      [{ name: 'recipient', value: recipient }],
      !recipient ? 'Add new recipient' : 'Edit recipient'
    );

    ref.instance.onSaveRecipient.subscribe((editedRecipient: E1RecipientMessage) => {
      !recipient
        ? this.onRecipientMessageCreated.emit(editedRecipient)
        : this.onRecipientMessageEdit.emit({ original: recipient, newValue: editedRecipient });

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  public deleteRecipientClick(recipient: E1RecipientMessage) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [{ name: 'message', value: `Are you sure you want to delete recipient "${recipient.recipient}"?` }],
      'Delete recipient'
    );

    ref.instance.onConfirm.subscribe(async () => {
      this.onRecipientMessageDeleted.emit(recipient);

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }
}
