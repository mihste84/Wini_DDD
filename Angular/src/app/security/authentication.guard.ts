import { inject } from '@angular/core';
import { AuthenticationService } from './authentication.service';

export const isAutheticatedGuard = (auth = inject(AuthenticationService)) => auth.isSignedIn();
