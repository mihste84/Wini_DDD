import { APP_INITIALIZER, ApplicationConfig, inject } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import {
  IPublicClientApplication,
  PublicClientApplication,
  InteractionType,
  BrowserCacheLocation,
  LogLevel,
  AzureCloudInstance,
} from '@azure/msal-browser';
import {
  MsalGuard,
  MsalInterceptor,
  MsalBroadcastService,
  MsalInterceptorConfiguration,
  MsalService,
  MSAL_GUARD_CONFIG,
  MSAL_INSTANCE,
  MSAL_INTERCEPTOR_CONFIG,
  MsalGuardConfiguration,
} from '@azure/msal-angular';
import { routes } from './app.routes';
import { environment } from '../environments/environment';
import { AuthenticationService } from './security/authentication.service';
import { ErrorInterceptor } from './shared/interceptors/error.interceptor';
import { ApiInterceptor } from './shared/interceptors/api.interceptor';
import { LoggerService, Severity } from './shared/services/logger.service';

function getSeverityFromLogLevel(logLevel: LogLevel) {
  switch (logLevel) {
    case LogLevel.Error:
      return Severity.Error;
    case LogLevel.Verbose:
      return Severity.Verbose;
    case LogLevel.Warning:
      return Severity.Warning;
    default:
      return Severity.Info;
  }
}

function MSALInstanceFactory(loggerService = inject(LoggerService)): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.msalConfig.auth.clientId,
      authority: environment.msalConfig.auth.authority,
      redirectUri: '/',
      azureCloudOptions: {
        azureCloudInstance: AzureCloudInstance.AzurePublic,
        tenant: environment.msalConfig.auth.tenant,
      },
      postLogoutRedirectUri: '/',
    },
    cache: {
      cacheLocation: BrowserCacheLocation.SessionStorage,
    },
    system: {
      loggerOptions: {
        loggerCallback: (logLevel: LogLevel, message: string) => {
          loggerService.log(message, getSeverityFromLogLevel(logLevel));
        },
        logLevel: LogLevel.Error,
        piiLoggingEnabled: true,
      },
    },
  });
}

function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();
  protectedResourceMap.set(environment.graphConfig.uri, environment.graphConfig.scopes);
  protectedResourceMap.set(environment.apiConfig.uri, environment.apiConfig.scopes);

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap,
  };
}

function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: [...environment.graphConfig.scopes, ...environment.apiConfig.scopes],
    },
    loginFailedRoute: '/login-failed',
  };
}

export function initAuthentication(authService: AuthenticationService) {
  return async () => {
    await authService.init();
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true },
    LoggerService,
    AuthenticationService,
    {
      provide: APP_INITIALIZER,
      useFactory: initAuthentication,
      deps: [AuthenticationService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: MSALGuardConfigFactory,
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory,
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService,
    provideHttpClient(withInterceptorsFromDi()),
    provideRouter(routes, withComponentInputBinding()),
  ],
};
