import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.css'],
  standalone: true
})
export class ConfirmComponent {
  @Input() public message: string = '';
  @Input() public returnObject?: any;
  @Input() public confirmBtnText: string = 'Confirm';
  @Input() public cancelBtnText: string = 'Cancel';
  @Output() public onConfirm = new EventEmitter<any>();
  @Output() public onCancel = new EventEmitter<void>();
}
