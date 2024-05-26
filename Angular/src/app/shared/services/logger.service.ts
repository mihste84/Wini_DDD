import { Injectable } from '@angular/core';

export enum Severity {
  Verbose = 0,
  Info = 1,
  Warning = 2,
  Error = 3,
}

@Injectable({
  providedIn: 'root',
})
export class LoggerService {
  constructor() {}

  public log(message: string, severity: Severity = Severity.Info): void {
    switch (severity) {
      case Severity.Verbose:
        return console.log(message);
      case Severity.Info:
        return console.info(message);
      case Severity.Warning:
        return console.warn(message);
      case Severity.Error:
        return console.error(message);
    }
  }
}
