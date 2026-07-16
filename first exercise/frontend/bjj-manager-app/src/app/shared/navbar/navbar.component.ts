import { Component, EventEmitter, Input, Output, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  @Input() showMenuButton = false;
  @Output() readonly menuToggle = new EventEmitter<void>();

  readonly isAuthenticated = this.authService.isAuthenticated;
  readonly userName = this.authService.userName;
  readonly avatarInitial = computed(() => (this.userName() ?? '?').charAt(0).toUpperCase());

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
