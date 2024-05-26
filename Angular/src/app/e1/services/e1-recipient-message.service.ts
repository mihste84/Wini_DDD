import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { E1RecipientMessage, SqlResult } from '../models/types';

@Injectable({
  providedIn: 'root',
})
export class E1RecipientMessageService {
  constructor(private http: HttpClient) {}

  public insertNewRecipientMessage(bookingId: number, body: E1RecipientMessage) {
    return this.http.post<SqlResult>(`booking/${bookingId}/recipient`, body);
  }

  public editRecipientMessage(bookingId: number, body: E1RecipientMessage) {
    return this.http.patch<SqlResult>(`booking/${bookingId}/recipient`, body);
  }

  public deleteRecipientMessage(bookingId: number, body: E1RecipientMessage) {
    return this.http.delete<SqlResult>(`booking/${bookingId}/recipient?recipient=${body.recipient}`);
  }
}
