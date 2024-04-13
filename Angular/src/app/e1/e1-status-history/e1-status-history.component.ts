import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { E1Status } from '../models/types';
import { WiniStatus } from '../models/wini-status';

@Component({
  selector: 'app-e1-status-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './e1-status-history.component.html',
  styleUrl: './e1-status-history.component.css',
})
export class E1StatusHistoryComponent {
  @Input({ required: true }) public statuses!: E1Status[];

  public getStatus(status: number) {
    return WiniStatus[status];
  }
}
