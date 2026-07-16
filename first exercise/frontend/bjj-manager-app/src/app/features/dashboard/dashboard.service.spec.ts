import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { DashboardService } from './dashboard.service';

describe('DashboardService', () => {
  let service: DashboardService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('fetches the dashboard summary', () => {
    const summary = { userName: 'Vinicius', totalTrainings: 3, totalTechniques: 2 };

    service.getSummary().subscribe((result) => expect(result).toEqual(summary));

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/dashboard`);
    expect(req.request.method).toBe('GET');
    req.flush(summary);
  });
});
