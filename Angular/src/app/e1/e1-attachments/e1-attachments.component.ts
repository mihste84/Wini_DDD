import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { E1Attachment } from '../models/types';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NotificationService, NotificationType } from '../../shared/services/notification.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';

@Component({
  selector: 'app-e1-attachments',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule, ModalComponent, ConfirmComponent],
  templateUrl: './e1-attachments.component.html',
  styleUrl: './e1-attachments.component.css',
})
export class E1AttachmentsComponent {
  @Input({ required: true }) public loading = false;
  @Input() public readonly = false;
  @Input({ required: true }) public attachments: E1Attachment[] = [];
  @Output() public onUploadAttachment = new EventEmitter<FileList>();
  @Output() public onDeleteAttachment = new EventEmitter<E1Attachment>();
  @Output() public onViewAttachment = new EventEmitter<E1Attachment>();
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  private maxFileSize = 1024 * 1024 * 5; // 5MB
  public maxFileCount = 5;
  public faTrash = faTrash;

  constructor(private notificationService: NotificationService) {}

  public onFileChange(event: Event) {
    if (this.loading || this.readonly) return;
    const element = event.currentTarget as HTMLInputElement;
    const fileList: FileList | null = element?.files;

    if (fileList?.length && this.areFilesValid(fileList)) this.onUploadAttachment.emit(fileList);
  }

  public getMbSize(size: number) {
    return new Number(size / 1024 / 1024).toFixed(2);
  }

  public onDeleteAttachmentClick(attachment: E1Attachment) {
    if (this.loading || this.readonly) return;

    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [{ name: 'message', value: `Are you sure you want to delete attachment?.` }],
      'Delete attachment'
    );

    ref.instance.onConfirm.subscribe(async () => {
      this.onDeleteAttachment.emit(attachment);

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  private areFilesValid(files: FileList): boolean {
    if (files.length + this.attachments.length > this.maxFileCount) {
      this.notificationService.addNotification(
        `You can upload up to ${this.maxFileCount} files.`,
        'Too many attachments',
        NotificationType.Error
      );
      return false;
    }

    for (let i = 0; i < files.length; i++) {
      if (files[i].size > this.maxFileSize) {
        this.notificationService.addNotification(
          `File ${files[i].name} is too big. Max file size is 5MB.`,
          'File too big',
          NotificationType.Error
        );
        return false;
      }
    }

    return true;
  }
}
