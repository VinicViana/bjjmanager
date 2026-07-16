import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { TechniqueService } from './technique.service';

describe('TechniqueService', () => {
  let service: TechniqueService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBaseUrl}/techniques`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(TechniqueService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('creates a technique with steps', () => {
    const input = { name: 'Armbar', position: 'Mount', description: 'Classic armbar', steps: ['Break posture'] };

    service.create(input).subscribe();

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(input);
    req.flush({});
  });

  it('adds media to an existing technique', () => {
    const media = { fileName: 'armbar.mp4', fileUrl: 'https://example.com/armbar.mp4', mediaType: 'Video' as const };

    service.addMedia('technique-1', media).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/technique-1/media`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(media);
    req.flush({});
  });
});
