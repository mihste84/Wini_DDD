import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormControlComponent } from '../../shared/components/form-control/form-control.component';

@Component({
  selector: 'app-e1-booking-header',
  standalone: true,
  imports: [ReactiveFormsModule, FormControlComponent],
  providers: [],
  templateUrl: './e1-booking-header.component.html',
  styleUrl: './e1-booking-header.component.css',
})
export class E1BookingHeaderComponent {
  @Input() public form!: FormGroup;
}
