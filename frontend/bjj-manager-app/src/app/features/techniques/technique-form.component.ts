import { Component, OnInit, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MediaUploadService } from '../../core/media/media-upload.service';
import { MediaItem } from '../../core/models/common.model';
import { POSITION_OPTIONS, TechniqueInput } from '../../core/models/technique.model';
import { TechniqueService } from './technique.service';

@Component({
  selector: 'app-technique-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './technique-form.component.html'
})
export class TechniqueFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly techniqueService = inject(TechniqueService);
  private readonly mediaUploadService = inject(MediaUploadService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  readonly positionOptions = POSITION_OPTIONS;
  readonly techniqueId = signal<string | null>(null);
  readonly media = signal<MediaItem[]>([]);
  readonly uploading = signal(false);
  readonly uploadProgress = signal(0);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    position: ['', Validators.required],
    description: ['', Validators.required],
    steps: this.fb.nonNullable.array([this.fb.nonNullable.control('', Validators.required)])
  });

  get steps(): FormArray<FormControl<string>> {
    return this.form.controls.steps;
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.techniqueId.set(id);
      this.techniqueService.getById(id).subscribe((technique) => {
        this.form.patchValue({
          name: technique.name,
          position: technique.position,
          description: technique.description
        });

        this.steps.clear();
        [...technique.steps]
          .sort((a, b) => a.order - b.order)
          .forEach((step) => this.steps.push(this.fb.nonNullable.control(step.description, Validators.required)));

        this.media.set(technique.media);
      });
    }
  }

  addStep(): void {
    this.steps.push(this.fb.nonNullable.control('', Validators.required));
  }

  removeStep(index: number): void {
    if (this.steps.length > 1) {
      this.steps.removeAt(index);
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const input: TechniqueInput = {
      name: value.name,
      position: value.position,
      description: value.description,
      steps: value.steps
    };

    const id = this.techniqueId();

    if (id) {
      this.techniqueService.update(id, input).subscribe(() => this.router.navigate(['/techniques']));
    } else {
      this.techniqueService.create(input).subscribe((created) => {
        this.techniqueId.set(created.id);
        this.media.set(created.media);
        this.router.navigate(['/techniques', created.id, 'edit'], { replaceUrl: true });
        this.snackBar.open('Technique created. You can now add photos and videos below.', 'Close', { duration: 5000 });
      });
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    input.value = '';

    const id = this.techniqueId();

    if (!file || !id || this.uploading()) {
      return;
    }

    this.uploading.set(true);
    this.uploadProgress.set(0);

    this.mediaUploadService.uploadFile('techniques', id, file).subscribe({
      next: ({ progress, downloadUrl }) => {
        this.uploadProgress.set(progress);

        if (downloadUrl) {
          const mediaType = MediaUploadService.mediaTypeFromFile(file);

          this.techniqueService
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
    const id = this.techniqueId();

    if (!id) {
      return;
    }

    this.techniqueService.removeMedia(id, mediaId).subscribe(() => {
      this.media.update((items) => items.filter((item) => item.id !== mediaId));
    });
  }
}
