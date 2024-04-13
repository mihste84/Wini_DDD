import { Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { firstValueFrom } from 'rxjs';

export interface AppUser {
  isAuthenticated: boolean;
  userName: string;
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private authService: MsalService) {}

  public isSignedIn() {
    return this.authService.instance.getActiveAccount() != null;
  }

  public getAppUser(): AppUser {
    const currentAccount = this.authService.instance.getActiveAccount();
    return {
      isAuthenticated: currentAccount !== null,
      userName: currentAccount?.username ?? 'N/A',
      name: currentAccount?.name ?? 'Not signed in',
    };
  }

  private checkAndSetActiveAccount() {
    let activeAccount = this.authService.instance?.getActiveAccount();

    if (!activeAccount && this.authService.instance.getAllAccounts().length > 0) {
      let accounts = this.authService.instance.getAllAccounts();
      this.authService.instance.setActiveAccount(accounts[0]);
    }
  }

  public async init() {
    const req = this.authService.handleRedirectObservable();
    await firstValueFrom(req);
    this.checkAndSetActiveAccount();

    this.authService.instance.addEventCallback((message) => {
      if (message.eventType === 'msal:acquireTokenFailure') this.authService.instance.loginRedirect();
    });
  }
}
