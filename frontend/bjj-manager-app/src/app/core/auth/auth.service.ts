import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse, RegisterResponse } from '../models/auth.model';

const TOKEN_KEY = 'bjjmanager.token';
const USER_NAME_KEY = 'bjjmanager.userName';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly userNameSignal = signal<string | null>(localStorage.getItem(USER_NAME_KEY));

  readonly userName = this.userNameSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.userNameSignal() !== null);

  register(name: string, password: string) {
    return this.http.post<RegisterResponse>(`${environment.apiBaseUrl}/auth/register`, { name, password });
  }

  login(name: string, password: string) {
    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, { name, password }).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_KEY, response.token);
        localStorage.setItem(USER_NAME_KEY, response.userName);
        this.userNameSignal.set(response.userName);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_NAME_KEY);
    this.userNameSignal.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }
}
