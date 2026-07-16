import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('starts unauthenticated when there is no stored session', () => {
    expect(service.isAuthenticated()).toBeFalse();
    expect(service.getToken()).toBeNull();
  });

  it('stores the token and user name and becomes authenticated after login', () => {
    service.login('Vinicius', 'Secret6').subscribe();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({ token: 'jwt-token', expiresAtUtc: new Date().toISOString(), userName: 'Vinicius' });

    expect(service.isAuthenticated()).toBeTrue();
    expect(service.userName()).toBe('Vinicius');
    expect(service.getToken()).toBe('jwt-token');
  });

  it('clears the session on logout', () => {
    service.login('Vinicius', 'Secret6').subscribe();
    httpMock.expectOne(`${environment.apiBaseUrl}/auth/login`).flush({
      token: 'jwt-token',
      expiresAtUtc: new Date().toISOString(),
      userName: 'Vinicius'
    });

    service.logout();

    expect(service.isAuthenticated()).toBeFalse();
    expect(service.getToken()).toBeNull();
  });
});
