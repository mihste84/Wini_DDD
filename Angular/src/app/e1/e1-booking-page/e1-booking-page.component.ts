import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-e1-booking-page',
  standalone: true,
  imports: [],
  templateUrl: './e1-booking-page.component.html',
  styleUrl: './e1-booking-page.component.css'
})
export class E1BookingPageComponent {
  @Input({ required: true }) public booking: any;

}
