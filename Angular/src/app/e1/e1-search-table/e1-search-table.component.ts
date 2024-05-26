import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BookingSearchResult } from '../models/types';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft, faArrowRight, faEdit, faPaperclip, faUpload } from '@fortawesome/free-solid-svg-icons';
import { RouterModule } from '@angular/router';
import { WiniStatus } from '../models/wini-status';
import { FormsModule } from '@angular/forms';
import { SortResult, TableHeaderComponent } from '../../shared/components/table-header/table-header.component';
import { AuthenticationService } from '../../security/authentication.service';

@Component({
  selector: 'app-e1-search-table',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule, RouterModule, FormsModule, TableHeaderComponent],
  templateUrl: './e1-search-table.component.html',
  styleUrl: './e1-search-table.component.css',
})
export class E1SearchTableComponent {
  @Input({ required: true }) public searchResults: BookingSearchResult[] = [];
  @Input({ required: true }) public currentPage = 0;
  @Input({ required: true }) public itemsPerPage = 25;
  @Input({ required: true }) public orderBy = 'Id';
  @Input({ required: true }) public orderDirection = 'ASC';
  @Input({ required: true }) public hasMorePages = false;
  @Input() public loading = false;

  @Output() public onExport = new EventEmitter<void>();
  @Output() public onSearch = new EventEmitter<number>();
  @Output() public onSort = new EventEmitter<SortResult>();
  @Output() public onUploadAttachment = new EventEmitter<{ booking: BookingSearchResult; files: FileList }>();

  public maxFileCount = 5;
  public faEdit = faEdit;
  public faPaperclip = faPaperclip;
  public faUpload = faUpload;
  public status = WiniStatus;
  public faArrowRight = faArrowRight;
  public faArrowLeft = faArrowLeft;
  private readonly userName: string;
  public tableHeaders = [
    { name: 'Id', title: '#', isSortable: true },
    { name: 'BookingDate', title: 'Booking date', isSortable: true },
    { name: 'CreatedBy', title: 'Commissioner', isSortable: true },
    { name: 'Status', title: 'Status', isSortable: true },
    { name: 'Comments', title: 'Comments', isSortable: false },
  ];

  constructor(auth: AuthenticationService) {
    this.userName = auth.getAppUser().userName;
  }

  public getComment(comment?: string): string | undefined {
    return comment?.replace(/\r\n/g, '<br>');
  }

  public getLink(booking: BookingSearchResult) {
    return booking.status === WiniStatus.Saved && booking.commissioner === this.userName
      ? `/e1/edit/${booking.bookingId}`
      : `/e1/view/${booking.bookingId}`;
  }

  public onFileChange(booking: BookingSearchResult, event: Event) {
    const element = event.currentTarget as HTMLInputElement;
    const files: FileList | null = element?.files;

    if (files?.length && this.canUploadFile(booking)) this.onUploadAttachment.emit({ booking, files });
  }

  public canUploadFile(booking: BookingSearchResult): boolean {
    return booking.status === WiniStatus.Saved && booking.commissioner === this.userName && booking.numberOfAttachments < this.maxFileCount;
  }
}
