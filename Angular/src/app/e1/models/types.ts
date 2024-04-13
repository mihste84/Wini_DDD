import { Ledger } from './ledger';
import { WiniStatus } from './wini-status';

export interface E1BookingHeader {
  bookingDate?: string;
  textToE1?: string;
  isReversed?: string;
  ledgerType?: number;
}

export interface E1BookingInput extends E1BookingHeader {
  rows?: E1BookingRow[];
}

export interface E1BookingRow {
  rowNumber: number;
  businessUnit?: string;
  account?: string;
  subsidiary?: string;
  subledger?: string;
  subledgerType?: string;
  costObject1?: string;
  costObjectType1?: string;
  costObject2?: string;
  costObjectType2?: string;
  costObject3?: string;
  costObjectType3?: string;
  costObject4?: string;
  costObjectType4?: string;
  remark?: string;
  authorizer?: string;
  isAuthorized?: boolean;
  amount?: number;
  currencyCode?: string;
  exchangeRate?: number;
  isNew?: boolean;
  toDelete?: boolean;
}

export interface BookingValidationError {
  errorCode: string;
  message: string;
  propertyName: string;
}

export interface BookingValidationResult {
  isValid: boolean;
  errors?: BookingValidationError[];
  message: string;
}

export interface BookingRowImport {
  businessUnit?: string;
  account?: string;
  subsidiary?: string;
  subledger?: string;
  subledgerType?: string;
  costObject?: string;
  costObjectType?: string;
  debit?: string;
  credit?: string;
  remark?: string;
  currency?: string;
  exchangeRate?: string;
  authorizer?: string;
  bookingGroup?: string;
  accountingDate?: string;
  textToE1?: string;
  emptyColumn1?: string;
  emptyColumn2?: string;
  useGPLedger?: string;
  costObject2?: string;
  costObjectType2?: string;
  costObject3?: string;
  costObjectType3?: string;
  costObject4?: string;
  costObjectType4?: string;
  glre?: string;
}

export interface SqlResult {
  id: number;
  rowVersion: string;
}

export interface E1Booking {
  bookingId: number;
  status: WiniStatus;
  bookingDate: string;
  textToE1?: string;
  reversed: boolean;
  ledgerType: Ledger;
  commissioner: string;
  updatedBy: string;
  updated: string;
  rowVersion: string;
  rows: E1BookingRow[];
  statusHistory: E1Status[];
  comments: E1Comment[];
  messages: E1RecipientMessage[];
  attachments: E1Attachment[];
  deletedRowNumbers: number[];
  maxRowNumber: number;
}

export interface E1Status {
  status: WiniStatus;
  updated: string;
  updatedBy: string;
}

export interface E1RecipientMessage {
  message: string;
  recipient: string;
}

export interface E1Attachment {
  size: number;
  contentType: string;
  name: string;
  path: string;
}

export interface E1Comment {
  value: string;
  created: string;
}
