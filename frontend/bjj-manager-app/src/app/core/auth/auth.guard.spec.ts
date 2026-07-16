import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, provideRouter, Router, RouterStateSnapshot } from '@angular/router';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  let router: Router;

  const runGuard = () =>
    TestBed.runInInjectionContext(() =>
      authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot)
    );

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    });

    router = TestBed.inject(Router);
  });

  afterEach(() => localStorage.clear());

  it('allows navigation when a session is already stored', () => {
    localStorage.setItem('bjjmanager.userName', 'Vinicius');

    expect(runGuard()).toBeTrue();
  });

  it('redirects to /login and blocks navigation when not authenticated', () => {
    spyOn(router, 'navigate');

    expect(runGuard()).toBeFalse();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
