export interface SearchResults<T> {
  items: T[];
  endRow: number;
  startRow: number;
  totalCount: number;
  orderByDirection: string;
  orderBy: string;
}

export interface SqlResult {
  id: number;
  rowVersion: string;
}
