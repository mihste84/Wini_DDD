import { Component, Input } from '@angular/core';
import { NotificationService } from '../../services/notification.service';
import { AppUser, AuthService } from '../../../security/auth.service';
import {
  faLightbulb as faLightbulbSolid,
  faRightToBracket,
  faUser,
  faRightFromBracket,
} from '@fortawesome/free-solid-svg-icons';
import { faLightbulb as faLightbulbRegular } from '@fortawesome/free-regular-svg-icons';
import { NgxPopperjsModule, NgxPopperjsPlacements } from 'ngx-popperjs';
import { environment } from '../../../../environments/environment';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NotificationListComponent } from '../notification-list/notification-list.component';
import { NavLinkComponent } from '../nav-link/nav-link.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule, NgxPopperjsModule, NotificationListComponent, NavLinkComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  @Input() public user?: AppUser;
  public popperBottom = NgxPopperjsPlacements.BOTTOMSTART;
  public title = environment.title;

  public faRightToBracket = faRightToBracket;
  public faLightbulbSolid = faLightbulbSolid;
  public faLightbulbRegular = faLightbulbRegular;
  public faRightFromBracket = faRightFromBracket;
  public faUser = faUser;

  constructor(public auth: AuthService, public notifications: NotificationService) {}
}
