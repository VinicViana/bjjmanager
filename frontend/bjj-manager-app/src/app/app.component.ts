import { Component, computed, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { map } from 'rxjs';
import { AuthService } from './core/auth/auth.service';
import { NavbarComponent } from './shared/navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NavbarComponent,
    MatSidenavModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly authService = inject(AuthService);
  private readonly breakpointObserver = inject(BreakpointObserver);

  readonly isAuthenticated = this.authService.isAuthenticated;

  readonly isHandset = toSignal(
    this.breakpointObserver.observe('(max-width: 768px)').pipe(map((result) => result.matches)),
    { initialValue: false }
  );

  readonly sidenavMode = computed<'over' | 'side'>(() => (this.isHandset() ? 'over' : 'side'));
  readonly sidenavOpened = signal(true);

  constructor() {
    effect(
      () => {
        this.sidenavOpened.set(!this.isHandset());
      },
      { allowSignalWrites: true }
    );
  }

  toggleSidenav(): void {
    this.sidenavOpened.update((opened) => !opened);
  }

  closeOnHandset(): void {
    if (this.isHandset()) {
      this.sidenavOpened.set(false);
    }
  }
}
