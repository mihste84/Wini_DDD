import { Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { firstValueFrom } from 'rxjs';
import { AppUser } from '../shared/models/types';
import { AuthorizationService } from './authorization.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  constructor(private authService: MsalService, private authorizationService: AuthorizationService) {}

  public isSignedIn() {
    return this.authService.instance.getActiveAccount() != null;
  }

  public async signOut() {
    const req = this.authService.logoutRedirect();
    await firstValueFrom(req);
  }

  public getAppUser(): AppUser {
    const currentAccount = this.authService.instance.getActiveAccount();
    return {
      isAuthenticated: currentAccount !== null,
      userName: currentAccount?.username ?? 'N/A',
      name: currentAccount?.name ?? 'Not signed in',
    };
  }

  public async init() {
    const req = this.authService.handleRedirectObservable();
    await firstValueFrom(req);
    this.checkAndSetActiveAccount();
    this.authService.instance.addEventCallback((message) => {
      if (message.eventType === 'msal:acquireTokenFailure') this.authService.instance.loginRedirect();
    });
    if (this.isSignedIn()) await this.authorizationService.init();
  }

  private checkAndSetActiveAccount() {
    const activeAccount = this.authService.instance?.getActiveAccount();

    if (!activeAccount && this.authService.instance.getAllAccounts().length > 0) {
      let accounts = this.authService.instance.getAllAccounts();
      this.authService.instance.setActiveAccount(accounts[0]);
    }
  }
}
