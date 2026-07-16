import { DatePipe, NgClass } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { formatDateOnly } from '../../core/utils/date.util';
import { SELF_EVALUATION_OPTIONS, SelfEvaluation, TrainingSession } from '../../core/models/training.model';
import { TrainingService } from './training.service';

@Component({
  selector: 'app-training-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DatePipe,
    NgClass,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule
  ],
  templateUrl: './training-list.component.html'
})
export class TrainingListComponent implements OnInit {
  private readonly trainingService = inject(TrainingService);
  private readonly fb = inject(FormBuilder);

  readonly trainings = signal<TrainingSession[]>([]);
  readonly displayedColumns = ['trainingDate', 'academyName', 'selfEvaluation', 'notes', 'actions'];

  readonly filterForm = this.fb.nonNullable.group({
    date: [null as Date | null],
    month: [null as number | null],
    year: [null as number | null]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    const { date, month, year } = this.filterForm.getRawValue();

    this.trainingService
      .getAll({ date: formatDateOnly(date) ?? undefined, month: month ?? undefined, year: year ?? undefined })
      .subscribe((trainings) => this.trainings.set(trainings));
  }

  clearFilters(): void {
    this.filterForm.reset({ date: null, month: null, year: null });
    this.load();
  }

  remove(id: string): void {
    this.trainingService.delete(id).subscribe(() => this.load());
  }

  evaluationLabel(value: SelfEvaluation): string {
    return SELF_EVALUATION_OPTIONS.find((option) => option.value === value)?.label ?? value;
  }
}
