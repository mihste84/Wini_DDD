import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { E1Comment, SqlResult } from '../models/types';

@Injectable({
  providedIn: 'root',
})
export class E1CommentService {
  constructor(private http: HttpClient) {}

  public insertNewComment(bookingId: number, body: E1Comment) {
    return this.http.post<SqlResult>(`booking/${bookingId}/comment`, body);
  }

  public editComment(bookingId: number, body: E1Comment) {
    return this.http.patch<SqlResult>(`booking/${bookingId}/comment`, body);
  }

  public deleteComment(bookingId: number, body: E1Comment) {
    return this.http.delete<SqlResult>(`booking/${bookingId}/comment?created=${body.created}`);
  }
}
