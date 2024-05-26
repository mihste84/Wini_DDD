import { inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface ComponentCanDeactivate {
  canDeactivate: () => boolean | Observable<boolean>;
}

export const canDeactivateComponent = (component: ComponentCanDeactivate) =>
  component.canDeactivate()
    ? true
    : confirm('WARNING: You have unsaved changes. Press Cancel to go back and save these changes, or OK to lose these changes.');
