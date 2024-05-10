import { Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { firstValueFrom } from 'rxjs';
import { AppUser, AppUserRights } from '../shared/models/types';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthorizationService {
  public isAdmin = false;
  public isRead = false;
  public isWrite = false;
  public isAccountingUser = false;
  public isControlActuary = false;
  public isSpecificActuary = false;
  public isBookingAuthorizationNeeded = false;

  constructor(private http: HttpClient) {}

  public async init() {
    const req = this.http.get<AppUserRights>('appuser');
    const rights = await firstValueFrom(req);

    this.isAdmin = rights.isAdmin;
    this.isRead = rights.isRead;
    this.isWrite = rights.isWrite;
    this.isAccountingUser = rights.isAccountingUser;
    this.isControlActuary = rights.isControlActuary;
    this.isSpecificActuary = rights.isSpecificActuary;
    this.isBookingAuthorizationNeeded = rights.isBookingAuthorizationNeeded;
  }
}
