import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { DashboardSummary } from '../../core/models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  getSummary() {
    return this.http.get<DashboardSummary>(`${environment.apiBaseUrl}/dashboard`);
  }
}
