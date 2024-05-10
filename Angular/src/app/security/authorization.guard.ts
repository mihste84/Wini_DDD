import { inject } from '@angular/core';
import { AuthorizationService } from './authorization.service';

export const isReadGuard = () => {
  const auth = inject(AuthorizationService);
  return auth.isRead;
};

export const isWriteGuard = () => {
  const auth = inject(AuthorizationService);
  return auth.isRead && auth.isWrite;
};
