import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { parseDateOnly, formatDateOnly } from '../../core/utils/date.util';
import { MediaUploadService } from '../../core/media/media-upload.service';
import { MediaItem } from '../../core/models/common.model';
import { SELF_EVALUATION_OPTIONS, SelfEvaluation, TrainingSessionInput } from '../../core/models/training.model';
import { TrainingService } from './training.service';

@Component({
  selector: 'app-training-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatProgressBarModule
  ],
  templateUrl: './training-form.component.html'
})
export class TrainingFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly trainingService = inject(TrainingService);
  private readonly mediaUploadService = inject(MediaUploadService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  readonly selfEvaluationOptions = SELF_EVALUATION_OPTIONS;
  readonly trainingId = signal<string | null>(null);
  readonly media = signal<MediaItem[]>([]);
  readonly uploading = signal(false);
  readonly uploadProgress = signal(0);

  readonly form = this.fb.nonNullable.group({
    trainingDate: [null as Date | null, Validators.required],
    academyName: ['', [Validators.required, Validators.maxLength(200)]],
    selfEvaluation: ['Average' as SelfEvaluation, Validators.required],
    notes: ['']
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.trainingId.set(id);
      this.trainingService.getById(id).subscribe((session) => {
        this.form.patchValue({
          trainingDate: parseDateOnly(session.trainingDate),
          academyName: session.academyName,
          selfEvaluation: session.selfEvaluation,
          notes: session.notes ?? ''
        });
        this.media.set(session.media);
      });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const input: TrainingSessionInput = {
      trainingDate: formatDateOnly(value.trainingDate)!,
      academyName: value.academyName,
      selfEvaluation: value.selfEvaluation,
      notes: value.notes || null
    };

    const id = this.trainingId();

    if (id) {
      this.trainingService.update(id, input).subscribe(() => this.router.navigate(['/trainings']));
    } else {
      this.trainingService.create(input).subscribe((created) => {
        this.trainingId.set(created.id);
        this.media.set(created.media);
        this.router.navigate(['/trainings', created.id, 'edit'], { replaceUrl: true });
        this.snackBar.open('Training created. You can now add photos and videos below.', 'Close', { duration: 5000 });
      });
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    input.value = '';

    const id = this.trainingId();

    if (!file || !id || this.uploading()) {
      return;
    }

    this.uploading.set(true);
    this.uploadProgress.set(0);

    this.mediaUploadService.uploadFile('trainings', id, file).subscribe({
      next: ({ progress, downloadUrl }) => {
        this.uploadProgress.set(progress);

        if (downloadUrl) {
          const mediaType = MediaUploadService.mediaTypeFromFile(file);

          this.trainingService
            .addMedia(id, { fileName: file.name, fileUrl: downloadUrl, mediaType })
            .subscribe({
              next: (media) => {
                this.media.update((items) => [...items, media]);
                this.uploading.set(false);
              },
              error: () => {
                this.uploading.set(false);
                this.snackBar.open('File uploaded but failed to save. Please try again.', 'Close', { duration: 5000 });
              }
            });
        }
      },
      error: () => {
        this.uploading.set(false);
        this.snackBar.open('Failed to upload file.', 'Close', { duration: 5000 });
      }
    });
  }

  removeMedia(mediaId: string): void {
    const id = this.trainingId();

    if (!id) {
      return;
    }

    this.trainingService.removeMedia(id, mediaId).subscribe(() => {
      this.media.update((items) => items.filter((item) => item.id !== mediaId));
    });
  }
}
