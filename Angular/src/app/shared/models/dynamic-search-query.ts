import { HttpParams } from '@angular/common/http';
import { SearchItem } from '../components/dynamic-search/dynamic-search.component';

export class DynamicSearchQuery {
  public orderBy: string = 'Id';
  public orderDirection: string = 'ASC';
  public searchItems: SearchItem[] = [];
  public itemsPerPage: number = 25;
  public page: number = 0;

  constructor(input: {
    itemsPerPage?: number;
    page?: number;
    orderBy?: string;
    orderByDirection?: string;
    searchItems?: SearchItem[];
  }) {
    this.orderBy = input.orderBy ?? this.orderBy;
    this.orderDirection = input.orderByDirection ?? this.orderDirection;
    this.searchItems = input.searchItems ?? this.searchItems;
    this.itemsPerPage = input.itemsPerPage ?? this.itemsPerPage;
    this.page = input.page ?? this.page;
  }

  public selectedSearchItemCount(): number {
    return this.searchItems.filter((_) => _.selected).length;
  }

  public getQueryParams(): HttpParams {
    let params = new HttpParams();
    const startRow = this.page * this.itemsPerPage;
    const endRow = startRow + this.itemsPerPage;
    params = params.set('startRow', startRow.toString());
    params = params.set('endRow', endRow.toString());
    params = params.set('orderBy', this.orderBy);
    params = params.set('orderByDirection', this.orderDirection);
    this.searchItems
      .filter((_) => _.selected)
      .forEach((item, index) => {
        params = params.append(`searchItems[${index}].name`, item.name);
        params = params.append(`searchItems[${index}].value`, item.value?.toString() ?? '');
        params = params.append(`searchItems[${index}].operator`, item.operator ?? '');
        params = params.append(
          `searchItems[${index}].handleAutomatically`,
          item.handleAutomatically?.toString() ?? 'true'
        );
      });
    return params;
  }
}
