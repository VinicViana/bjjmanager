import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { MediaInput, MediaItem } from '../../core/models/common.model';
import { Technique, TechniqueInput } from '../../core/models/technique.model';

@Injectable({ providedIn: 'root' })
export class TechniqueService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/techniques`;

  getAll() {
    return this.http.get<Technique[]>(this.baseUrl);
  }

  getById(id: string) {
    return this.http.get<Technique>(`${this.baseUrl}/${id}`);
  }

  create(input: TechniqueInput) {
    return this.http.post<Technique>(this.baseUrl, input);
  }

  update(id: string, input: TechniqueInput) {
    return this.http.put<void>(`${this.baseUrl}/${id}`, input);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  addMedia(techniqueId: string, media: MediaInput) {
    return this.http.post<MediaItem>(`${this.baseUrl}/${techniqueId}/media`, media);
  }

  removeMedia(techniqueId: string, mediaId: string) {
    return this.http.delete<void>(`${this.baseUrl}/${techniqueId}/media/${mediaId}`);
  }
}
