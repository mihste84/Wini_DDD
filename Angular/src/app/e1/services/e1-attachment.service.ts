import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SqlResult } from '../models/types';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root',
})
export class E1AttachmentService {
  constructor(private http: HttpClient) {}

  public uploadAttachment(bookingId: number, files: FileList) {
    const formData = new FormData();
    Array.from(files).forEach((file) => formData.append('uploadedFiles', file));
    return this.http.post<SqlResult>(`booking/${bookingId}/attachment`, formData);
  }

  public deleteComment(bookingId: number, fileName: string) {
    return this.http.delete<SqlResult>(`booking/${bookingId}/attachment?fileName=${fileName}`);
  }

  public downloadFile(bookingId: number, fileName: string) {
    this.http.get(`booking/${bookingId}/attachment?fileName=${fileName}`, { responseType: 'blob' }).subscribe((_) => {
      saveAs(_, fileName);
    });
  }
}
