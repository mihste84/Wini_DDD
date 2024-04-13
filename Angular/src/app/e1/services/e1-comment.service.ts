import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { E1Comment, SqlResult } from '../models/types';

@Injectable({
  providedIn: 'root',
})
export class E1CommentService {
  constructor(private http: HttpClient) {}

  public insertNewComment(bookingId: number, rowVersion: string, body: E1Comment) {
    return this.http.post<SqlResult>(`booking/${bookingId}/comment`, body, {
      headers: { rowVersion: rowVersion },
    });
  }

  public editComment(bookingId: number, rowVersion: string, body: E1Comment) {
    return this.http.patch<SqlResult>(`booking/${bookingId}/comment`, body, {
      headers: { rowVersion: rowVersion },
    });
  }

  public deleteComment(bookingId: number, rowVersion: string, body: E1Comment) {
    return this.http.delete<SqlResult>(`booking/${bookingId}/comment?created=${body.created}`, {
      headers: { rowVersion: rowVersion },
    });
  }
}
