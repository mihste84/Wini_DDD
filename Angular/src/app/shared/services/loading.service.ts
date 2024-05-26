import { Injectable, computed, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  public isLoading = computed(() => this.ongoingRequests() > 0);
  public ongoingRequests = signal<number>(0);

  public startLoading() {
    this.ongoingRequests.update((_) => _ + 1);
  }

  public stopLoading() {
    if (this.ongoingRequests() > 0) this.ongoingRequests.update((_) => _ - 1);
  }
}
