import { Routes } from '@angular/router';
import { e1ExistingBookingResolver } from './e1-existing-booking.resolver';
import { isReadGuard, isWriteGuard } from '../security/authorization.guard';
import { canDeactivateComponent } from '../shared/guards/can-deactivate.guard';

export const e1Routes: Routes = [
  {
    path: '',
    redirectTo: 'search',
    pathMatch: 'full',
  },
  {
    path: 'search',
    loadComponent: () => import('./e1-search-page/e1-search-page.component').then((m) => m.E1SearchPageComponent),
  },
  {
    path: 'new',
    loadComponent: () => import('./e1-new-booking-page/e1-new-booking-page.component').then((m) => m.E1NewBookingPageComponent),
    canActivate: [isWriteGuard],
    canDeactivate: [canDeactivateComponent],
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./e1-booking-page/e1-booking-page.component').then((m) => m.E1BookingPageComponent),
    resolve: { booking: e1ExistingBookingResolver },
    canActivate: [isWriteGuard],
    canDeactivate: [canDeactivateComponent],
  },
  {
    path: 'view/:id',
    loadComponent: () => import('./e1-booking-view-page/e1-booking-view-page.component').then((m) => m.E1BookingViewPageComponent),
    resolve: { booking: e1ExistingBookingResolver },
    canActivate: [isReadGuard],
  },
];
