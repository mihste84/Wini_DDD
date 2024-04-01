import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const isAutheticatedGuard = (auth = inject(AuthService)) => auth.isSignedIn();

