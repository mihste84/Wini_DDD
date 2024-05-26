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

export interface AppUserRights {
  isAdmin: boolean;
  isRead: boolean;
  isWrite: boolean;
  isAccountingUser: boolean;
  isControlActuary: boolean;
  isSpecificActuary: boolean;
  isBookingAuthorizationNeeded: boolean;
}

export interface AppUser {
  isAuthenticated: boolean;
  userName: string;
  name: string;
}

export interface SearchResultWrapper<T> {
  hasMorePages: boolean;
  items: T[];
  nextStartRow: number;
  nextEndRow: number;
}
