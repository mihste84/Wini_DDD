import { Component, NO_ERRORS_SCHEMA, effect } from '@angular/core';
import { DynamicSearchQuery } from '../../shared/models/dynamic-search-query';
import { DynamicSearchComponent } from '../../shared/components/dynamic-search/dynamic-search.component';
import { E1BookingService } from '../services/e1-booking.service';
import { firstValueFrom } from 'rxjs';
import { LoadingService } from '../../shared/services/loading.service';
import { E1SearchTableComponent } from '../e1-search-table/e1-search-table.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { SortResult } from '../../shared/components/table-header/table-header.component';
import { NotificationService, NotificationType } from '../../shared/services/notification.service';
import { E1SearchBookingService } from '../services/e1-search-booking.service';
import { E1AttachmentService } from '../services/e1-attachment.service';
import { BookingSearchResult } from '../models/types';

@Component({
  schemas: [NO_ERRORS_SCHEMA],
  selector: 'app-e1-search-page',
  standalone: true,
  templateUrl: './e1-search-page.component.html',
  styleUrl: './e1-search-page.component.css',
  imports: [DynamicSearchComponent, E1SearchTableComponent, FontAwesomeModule],
})
export class E1SearchPageComponent {
  public loading = false;

  constructor(
    public searchService: E1SearchBookingService,
    private bookingService: E1BookingService,
    private notificationService: NotificationService,
    private attachemtService: E1AttachmentService,
    loadingService: LoadingService
  ) {
    effect(() => {
      this.loading = loadingService.isLoading();
    });
  }

  public async onExportCallback() {}

  public async onSearchClick() {
    if (this.loading) return;

    this.searchService.searchModel.page = 0;
    const req = this.bookingService.searchBookings(this.searchService.searchModel);
    const res = await firstValueFrom(req);
    this.searchService.searchResults = res.items;
    this.searchService.hasMorePages = res.hasMorePages;
  }

  public onSortCallback(sortResult: SortResult) {
    this.searchService.searchModel.orderBy = sortResult.columnName;
    this.searchService.searchModel.orderDirection = sortResult.direction;
  }

  public async onPageChangeCallback(page: number) {
    if (this.loading) return;

    const tmpModel = new DynamicSearchQuery(this.searchService.searchModel); // Dont update searchModel yet
    tmpModel.page = page;
    const req = this.bookingService.searchBookings(tmpModel);
    const res = await firstValueFrom(req);
    if (!res.items.length) {
      // Dont change page if no data is returned. Most likely last page for current search criteria.
      tmpModel.page = this.searchService.searchModel.page;
      this.searchService.hasMorePages = false;
      this.notificationService.addNotification('No more data to load', 'Search bookings', NotificationType.Info);
    } else {
      this.searchService.searchResults = res.items;
      this.searchService.hasMorePages = res.hasMorePages;
    }

    this.searchService.searchModel = tmpModel;
  }

  public async uploadAttachmentCallback(item: { booking: BookingSearchResult; files: FileList }) {
    if (this.loading) return;
    const bookingId = item.booking.bookingId;
    const req = this.attachemtService.uploadAttachment(bookingId, item.files);
    const result = await firstValueFrom(req);

    this.notificationService.addNotification(
      'Attachments uploaded.',
      `Attachment uploaded to booking ${bookingId}.`,
      NotificationType.Info
    );
    item.booking.numberOfAttachments = item.booking.numberOfAttachments + 1;
  }
}
