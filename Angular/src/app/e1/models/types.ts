export interface E1BookingHeader {
  bookingDate?: Date;
  textToE1?: string;
  isReversed?: string;
  ledgerType?: string;
};

export interface NewE1Booking extends E1BookingHeader {
  rows?: NewE1BookingRow[];
};

export interface NewE1BookingRow {
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
  amount?: number;
  currencyCode?: string;
  exchangeRate?: number;
};
