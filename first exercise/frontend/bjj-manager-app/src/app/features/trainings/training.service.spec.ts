import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { TrainingService } from './training.service';

describe('TrainingService', () => {
  let service: TrainingService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBaseUrl}/trainings`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(TrainingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('sends only the filters that are set', () => {
    service.getAll({ month: 7 }).subscribe();

    const req = httpMock.expectOne((r) => r.url === baseUrl);
    expect(req.request.params.has('date')).toBeFalse();
    expect(req.request.params.get('month')).toBe('7');
    expect(req.request.params.has('year')).toBeFalse();
    req.flush([]);
  });

  it('creates a training session', () => {
    const input = { trainingDate: '2026-01-10', academyName: 'Gracie Barra', selfEvaluation: 'Good' as const, notes: null };

    service.create(input).subscribe();

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(input);
    req.flush({});
  });

  it('deletes a training session', () => {
    service.delete('training-1').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/training-1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
