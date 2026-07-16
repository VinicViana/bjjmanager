import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { MediaInput, MediaItem } from '../../core/models/common.model';
import { TrainingFilter, TrainingNotesSummary, TrainingSession, TrainingSessionInput } from '../../core/models/training.model';

@Injectable({ providedIn: 'root' })
export class TrainingService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/trainings`;

  getAll(filter: TrainingFilter = {}) {
    let params = new HttpParams();

    if (filter.date) {
      params = params.set('date', filter.date);
    }
    if (filter.month) {
      params = params.set('month', filter.month);
    }
    if (filter.year) {
      params = params.set('year', filter.year);
    }

    return this.http.get<TrainingSession[]>(this.baseUrl, { params });
  }

  getById(id: string) {
    return this.http.get<TrainingSession>(`${this.baseUrl}/${id}`);
  }

  getNotesSummary() {
    return this.http.get<TrainingNotesSummary>(`${this.baseUrl}/notes-summary`);
  }

  create(input: TrainingSessionInput) {
    return this.http.post<TrainingSession>(this.baseUrl, input);
  }

  update(id: string, input: TrainingSessionInput) {
    return this.http.put<void>(`${this.baseUrl}/${id}`, input);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  addMedia(trainingId: string, media: MediaInput) {
    return this.http.post<MediaItem>(`${this.baseUrl}/${trainingId}/media`, media);
  }

  removeMedia(trainingId: string, mediaId: string) {
    return this.http.delete<void>(`${this.baseUrl}/${trainingId}/media/${mediaId}`);
  }
}
