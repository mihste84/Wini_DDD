import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { FormErrorComponent } from '../form-error/form-error.component';

@Component({
  selector: 'app-form-control',
  templateUrl: './form-control.component.html',
  styleUrls: ['./form-control.component.css'],
  standalone: true,
  imports:[CommonModule, FormErrorComponent]
})
export class FormControlComponent {
  @Input() public label?: string;
  @Input() public name?: string;
  @Input() public control: AbstractControl<any, any> | null = null;
}
