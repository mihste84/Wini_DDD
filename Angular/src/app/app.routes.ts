import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { e1Routes } from './e1/e1.routes';
import { isReadGuard } from './security/authorization.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    canActivate: [MsalGuard, isReadGuard],
    loadComponent: () => import('./home/home-page/home-page.component').then((_) => _.HomePageComponent),
  },
  {
    path: 'e1',
    canActivateChild: [MsalGuard, isReadGuard],
    children: e1Routes,
  },
  {
    path: 'login-failed',
    loadComponent: () => import('./shared/components/login-failed/login-failed.component').then((_) => _.LoginFailedComponent),
  },
  {
    path: '**',
    loadComponent: () => import('./shared/components/not-found-page/not-found-page.component').then((_) => _.NotFoundPageComponent),
  },
];
