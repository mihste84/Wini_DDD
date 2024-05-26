import { Injectable } from '@angular/core';
import { LoggerService, Severity } from './logger.service';
import { NotificationService, NotificationType } from './notification.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ErrorHandlerService {
  constructor(private logger: LoggerService, private notifications: NotificationService) {}

  public addError(value: Error) {
    const error = this.resolveHttpError(value) ?? [
      { message: value.message, title: value.name },
      'A client error occurred.',
    ];
    this.logger.log(JSON.stringify(value), Severity.Error);
    this.notifications.addNotification(error[0].message, error[0].title, NotificationType.Error);
  }

  private resolveHttpError(error: Error): [{ message: string; title: string }, string] | undefined {
    if (!(error instanceof HttpErrorResponse)) return;

    let returnObj = {
      message: error.error?.message ?? (error.error as string) ?? error.message,
      title: 'Server error',
    };
    let logMsg = 'Server error, url=' + error.url;
    const validationErrorMsg = this.tryGetValidationErrorMsg(error.error) ?? error.error;
    switch (error.status) {
      case 0:
        logMsg = 'Server not reachable, url=' + error.url;
        returnObj = { message: 'The server is not reachable.', title: 'Server not reachable' };
        break;
      case 400:
        logMsg = 'Bad request, url=' + error.url + ', validationErrors=' + validationErrorMsg;
        returnObj = { message: validationErrorMsg ?? error.error, title: 'Validation error' };
        break;

      case 401:
        logMsg = 'Unauthenticated request, url=' + error.url;
        returnObj = { message: 'You are not allowed to access the requested resource.', title: 'Authentication error' };
        break;

      case 404:
        logMsg = 'Resource not found, url=' + error.url;
        returnObj = { message: 'The requested resource was not found.', title: 'Resource not found' };
        break;
    }

    return [returnObj, logMsg];
  }

  private tryGetValidationErrorMsg(validationError?: { errors: { errorMessage: string; propertyName: string }[] }) {
    return validationError?.errors?.map((_) => _.errorMessage)?.join('');
  }
}
