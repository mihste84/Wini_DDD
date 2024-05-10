import { Component, OnDestroy, effect } from '@angular/core';
import { EventType, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { AuthenticationService } from './security/authentication.service';
import { CommonModule } from '@angular/common';
import { BreakpointObserver, Breakpoints, LayoutModule } from '@angular/cdk/layout';
import { environment } from '../environments/environment';
import { LoadingService } from './shared/services/loading.service';
import { SideNavComponent } from './shared/components/side-nav/side-nav.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { Subscription, filter, map } from 'rxjs';
import { AppUser } from './shared/models/types';
import { AuthorizationService } from './security/authorization.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, LayoutModule, SideNavComponent, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnDestroy {
  public title = environment.title;
  public loading: boolean = false;
  public user?: AppUser;
  public showSideNav: boolean = true;
  public isSmallScreen: boolean = false;
  public urlType?: string;
  private sub: Subscription;

  constructor(
    authService: AuthenticationService,
    loadingService: LoadingService,
    responsive: BreakpointObserver,
    route: Router,
    public authorizationService: AuthorizationService
  ) {
    this.user = authService.getAppUser();
    effect(() => {
      this.loading = loadingService.isLoading();
    });
    responsive.observe([Breakpoints.Small, Breakpoints.XSmall, Breakpoints.Handset]).subscribe((result) => {
      this.isSmallScreen = result.matches;
      this.showSideNav = !result.matches;
    });

    this.sub = route.events
      .pipe(
        filter((_) => _.type == EventType.NavigationEnd),
        map((_) => _ as NavigationEnd),
        map((_) => _.urlAfterRedirects)
      )
      .subscribe((_) => {
        this.urlType = this.getUrlType(_);
      });
  }

  public toggleSideNavCallback() {
    this.showSideNav = !this.showSideNav;
  }

  private getUrlType(url: string) {
    if (!url) return;

    if (url.indexOf('e1') > -1) return 'e1';

    return url.indexOf('lh') > -1 ? 'lh' : undefined;
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
