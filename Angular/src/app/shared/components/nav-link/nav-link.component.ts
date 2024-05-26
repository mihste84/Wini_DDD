import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

@Component({
  selector: 'app-nav-link',
  templateUrl: './nav-link.component.html',
  styleUrls: ['./nav-link.component.css'],
  standalone: true,
  imports: [CommonModule, FontAwesomeModule, RouterModule]
})
export class NavLinkComponent {
  @Input({ required: true }) public link?: string;
  @Input({ required: true }) public linkText?: string;
  @Input() public icon?: IconProp;
  @Input({ required: true }) public showSideNav?: boolean;
}
