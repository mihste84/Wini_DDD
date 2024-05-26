import { Injectable } from '@angular/core';
import { DynamicSearchQuery } from '../../shared/models/dynamic-search-query';
import { SearchTypes } from '../../shared/components/dynamic-search/dynamic-search.component';
import { WiniStatus } from '../models/wini-status';
import { BookingSearchResult } from '../models/types';

@Injectable({
  providedIn: 'root',
})
export class E1SearchBookingService {
  public searchModel: DynamicSearchQuery = new DynamicSearchQuery({
    searchItems: [
      { name: 'Id', distplayName: 'ID', type: SearchTypes.Number },
      { name: 'FromBookingDate', distplayName: 'From booking date', type: SearchTypes.Date, handleAutomatically: false },
      { name: 'ToBookingDate', distplayName: 'To booking date', type: SearchTypes.Date, handleAutomatically: false },
      { name: 'FromCreatedDate', distplayName: 'From created date', type: SearchTypes.Date, handleAutomatically: false },
      { name: 'ToCreatedDate', distplayName: 'To created date', type: SearchTypes.Date, handleAutomatically: false },
      {
        name: 'Status',
        distplayName: 'Status',
        type: SearchTypes.Number,
        listValues: [
          { value: WiniStatus.Saved.toString(), displayName: 'Saved' },
          { value: WiniStatus.Cancelled.toString(), displayName: 'Cancelled' },
          { value: WiniStatus.NotAuthorizedOnTime.toString(), displayName: 'NotAuthorizedOnTime' },
          { value: WiniStatus.SendError.toString(), displayName: 'Send error' },
          { value: WiniStatus.Sent.toString(), displayName: 'Sent' },
          { value: WiniStatus.ToBeAuthorized.toString(), displayName: 'To be authorized' },
          { value: WiniStatus.ToBeSent.toString(), displayName: 'To be sent' },
        ],
        isValueSelectable: true,
      },
      { name: 'BusinessUnit', distplayName: 'Business unit', type: SearchTypes.Text, handleAutomatically: false },
      { name: 'Account', distplayName: 'Account', type: SearchTypes.Text, handleAutomatically: false },
      { name: 'CreatedBy', distplayName: 'Commissioner', type: SearchTypes.Text },
      { name: 'Authorizer', distplayName: 'Authorizer', type: SearchTypes.Text, handleAutomatically: false },
      { name: 'Amount', distplayName: 'Amount', type: SearchTypes.Text, handleAutomatically: false },
    ],
  });
  public searchResults: BookingSearchResult[] = [];
  public hasMorePages = false;
}
