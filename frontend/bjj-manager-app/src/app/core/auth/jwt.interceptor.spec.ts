import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { jwtInterceptor } from './jwt.interceptor';

describe('jwtInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let authService: AuthService;
  let router: Router;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([jwtInterceptor])),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('attaches the bearer token when one is stored', () => {
    spyOn(authService, 'getToken').and.returnValue('stored-token');

    http.get('/api/trainings').subscribe();

    const req = httpMock.expectOne('/api/trainings');
    expect(req.request.headers.get('Authorization')).toBe('Bearer stored-token');
    req.flush([]);
  });

  it('does not attach a header when there is no token', () => {
    http.get('/api/trainings').subscribe();

    const req = httpMock.expectOne('/api/trainings');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush([]);
  });

  it('logs out and redirects to /login on a 401 response', () => {
    spyOn(authService, 'logout');
    spyOn(router, 'navigate');

    http.get('/api/trainings').subscribe({ error: () => undefined });

    httpMock.expectOne('/api/trainings').flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
