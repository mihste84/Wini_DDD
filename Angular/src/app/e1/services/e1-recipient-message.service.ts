import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { E1RecipientMessage, SqlResult } from '../models/types';

@Injectable({
  providedIn: 'root',
})
export class E1RecipientMessageService {
  constructor(private http: HttpClient) {}

  public insertNewComment(bookingId: number, body: E1RecipientMessage) {
    return this.http.post<SqlResult>(`booking/${bookingId}/recipient`, body);
  }

  public editComment(bookingId: number, body: E1RecipientMessage) {
    return this.http.patch<SqlResult>(`booking/${bookingId}/recipient`, body);
  }

  public deleteComment(bookingId: number, body: E1RecipientMessage) {
    return this.http.delete<SqlResult>(`booking/${bookingId}/recipient?recipient=${body.recipient}`);
  }
}
