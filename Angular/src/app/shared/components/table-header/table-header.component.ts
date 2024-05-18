import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowDownWideShort, faArrowDownShortWide } from '@fortawesome/free-solid-svg-icons';
export interface SortResult {
  columnName: string;
  direction: string;
}

@Component({
  selector: 'app-table-header',
  templateUrl: './table-header.component.html',
  styleUrls: ['./table-header.component.css'],
  standalone: true,
  imports: [CommonModule, FontAwesomeModule],
})
export class TableHeaderComponent {
  @Input({ required: true }) public name: string = '';
  @Input() public currentSortColumnName?: string;
  @Input() public currentSortDirection?: string;
  @Input() public isSortable?: boolean = true;
  @Output() public onSort = new EventEmitter<SortResult>();

  public faArrowDownShortWide = faArrowDownShortWide;
  public faArrowDownWideShort = faArrowDownWideShort;

  public sort(): void {
    if (!this.isSortable) return;

    let direction = 'ASC';
    if (this.currentSortColumnName === this.name) direction = this.currentSortDirection === 'ASC' ? 'DESC' : 'ASC';
    this.onSort.emit({
      columnName: this.name,
      direction: direction,
    });
  }
}
