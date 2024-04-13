import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { E1Comment } from '../models/types';

@Component({
  selector: 'app-e1-comment-edit',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './e1-comment-edit.component.html',
  styleUrl: './e1-comment-edit.component.css',
})
export class E1CommentEditComponent implements OnInit {
  @Input({ required: true }) public comment!: E1Comment;
  @Output() public onEditComment = new EventEmitter<string>();
  @Output() public onCancel = new EventEmitter<void>();

  public form = new FormControl('', [Validators.required, Validators.maxLength(300)]);

  public onSubmit() {
    if (this.form.invalid) return;

    this.onEditComment.emit(this.form.value ?? '');
  }
  ngOnInit(): void {
    this.form.setValue(this.comment.value);
  }
}
