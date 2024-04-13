import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { E1Comment } from '../models/types';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash, faPencil } from '@fortawesome/free-solid-svg-icons';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';
import { E1CommentService } from '../services/e1-comment.service';
import { getFormattedDateTimeString } from '../../shared/utils/date.utils';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';

@Component({
  selector: 'app-e1-comments',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule, ReactiveFormsModule, FormControlComponent, ModalComponent, ConfirmComponent],
  templateUrl: './e1-comments.component.html',
  styleUrl: './e1-comments.component.css',
})
export class E1CommentsComponent {
  @Input({ required: true }) public comments: E1Comment[] = [];
  @Input() public loading: boolean = false;
  @Output() public onCommentCreated = new EventEmitter<E1Comment>();
  @Output() public onCommentDeleted = new EventEmitter<E1Comment>();
  @Output() public onCommentEdit = new EventEmitter<E1Comment>();
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public form = new FormControl('', [Validators.required, Validators.maxLength(300)]);
  public faTrash = faTrash;
  public faPencil = faPencil;

  constructor(public commentService: E1CommentService) {}

  public onDeleteClick(comment: E1Comment) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [{ name: 'message', value: `Are you sure you want to delete comment?` }],
      'Delete comment'
    );

    ref.instance.onConfirm.subscribe(async () => {
      this.onCommentDeleted.emit(comment);

      this.sharedModal!.hideModal();
    });

    ref.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  public getComments() {
    return this.comments.slice().sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());
  }

  public async onSubmit() {
    if (this.form.invalid) return;
    const comment: E1Comment = {
      value: this.form.value ?? '',
      created: getFormattedDateTimeString(new Date()),
    };

    this.onCommentCreated.emit(comment);
    this.form.reset();
  }
}
