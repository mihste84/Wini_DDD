import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-black-background',
  templateUrl: './black-background.component.html',
  styleUrls: ['./black-background.component.css'],
  standalone: true,
  imports:[CommonModule]
})
export class BlackBackgroundComponent {
  @Input({ required: true }) public show: boolean = false;
}
