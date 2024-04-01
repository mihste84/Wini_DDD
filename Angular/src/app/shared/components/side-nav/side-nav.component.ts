import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faAnglesLeft,
  faAnglesRight,
  faMagnifyingGlass,
  faPlus,
  faFileImport,
} from '@fortawesome/free-solid-svg-icons';
import { NavLinkComponent } from '../nav-link/nav-link.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.component.html',
  styleUrls: ['./side-nav.component.css'],
  standalone: true,
  imports: [FontAwesomeModule, CommonModule, NavLinkComponent, RouterModule],
})
export class SideNavComponent {
  @Input({ required: true }) public showSideNav?: boolean;
  @Input({ required: true }) public isSmallScreen?: boolean;
  @Input({ required: true }) public urlType?: string;

  @Output() public onToggleSideNav = new EventEmitter<void>();

  public faAnglesRight = faAnglesRight;
  public faAnglesLeft = faAnglesLeft;
  public faMagnifyingGlass = faMagnifyingGlass;
  public faPlus = faPlus;
  public faFileImport = faFileImport;
}
