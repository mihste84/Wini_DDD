import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-form-error',
  templateUrl: './form-error.component.html',
  styleUrls: ['./form-error.component.css'],
  standalone:true,
  imports: [CommonModule]
})
export class FormErrorComponent {
  @Input() public control: AbstractControl<any, any> | null = null;
}
