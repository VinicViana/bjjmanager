import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { DashboardSummary } from '../../core/models/dashboard.model';
import { DailyNote, TrainingNotesSummary } from '../../core/models/training.model';
import { TrainingService } from '../trainings/training.service';
import { DashboardService } from './dashboard.service';

interface ChartPoint {
  x: number;
  y: number;
  labelX: number;
  labelY: number;
  note: DailyNote;
}

const CHART_WIDTH = 640;
const CHART_HEIGHT = 200;
const PADDING_X = 24;
const PADDING_TOP = 20;
const PADDING_BOTTOM = 32;
const HALF_DAY_MS = 12 * 60 * 60 * 1000;

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule, MatIconModule, DatePipe, DecimalPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  private readonly trainingService = inject(TrainingService);

  readonly summary = signal<DashboardSummary | null>(null);
  readonly notesSummary = signal<TrainingNotesSummary | null>(null);

  readonly chartWidth = CHART_WIDTH;
  readonly chartHeight = CHART_HEIGHT;
  readonly axisY = CHART_HEIGHT - PADDING_BOTTOM;

  readonly chartPoints = computed<ChartPoint[]>(() => {
    const dailyNotes = this.notesSummary()?.dailyNotes ?? [];
    return this.toChartPoints(dailyNotes);
  });

  readonly linePath = computed(() => {
    const points = this.chartPoints();
    if (points.length === 0) {
      return '';
    }

    return points.map((p, i) => `${i === 0 ? 'M' : 'L'}${p.x.toFixed(1)},${p.y.toFixed(1)}`).join(' ');
  });

  readonly areaPath = computed(() => {
    const points = this.chartPoints();
    if (points.length === 0) {
      return '';
    }

    const first = points[0];
    const last = points[points.length - 1];
    const line = points.map((p) => `L${p.x.toFixed(1)},${p.y.toFixed(1)}`).join(' ');

    return `M${first.x.toFixed(1)},${this.axisY} ${line} L${last.x.toFixed(1)},${this.axisY} Z`;
  });

  readonly averageLineY = computed(() => {
    const ns = this.notesSummary();

    if (!ns || ns.totalSessions === 0) {
      return null;
    }

    return this.scoreToY(ns.overallAverage);
  });

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe((summary) => this.summary.set(summary));
    this.trainingService.getNotesSummary().subscribe((notesSummary) => this.notesSummary.set(notesSummary));
  }

  private toChartPoints(dailyNotes: DailyNote[]): ChartPoint[] {
    if (dailyNotes.length === 0) {
      return [];
    }

    const times = dailyNotes.map((n) => new Date(n.date).getTime());
    const minTime = Math.min(...times);
    const maxTime = Math.max(...times);
    const paddedMin = minTime === maxTime ? minTime - HALF_DAY_MS : minTime;
    const paddedMax = minTime === maxTime ? maxTime + HALF_DAY_MS : maxTime;
    const span = paddedMax - paddedMin;

    const usableWidth = CHART_WIDTH - PADDING_X * 2;

    return dailyNotes.map((note) => {
      const noteTime = new Date(note.date).getTime();
      const ratio = span === 0 ? 0.5 : (noteTime - paddedMin) / span;
      const x = PADDING_X + ratio * usableWidth;
      const y = this.scoreToY(note.averageScore);
      return { x, y, labelX: x, labelY: this.axisY + 16, note };
    });
  }

  private scoreToY(score: number): number {
    const usableHeight = CHART_HEIGHT - PADDING_TOP - PADDING_BOTTOM;
    return PADDING_TOP + (1 - score / 5) * usableHeight;
  }
}
