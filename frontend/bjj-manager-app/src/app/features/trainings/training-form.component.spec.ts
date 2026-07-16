import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNativeDateAdapter } from '@angular/material/core';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { TrainingFormComponent } from './training-form.component';

describe('TrainingFormComponent', () => {
  let fixture: ComponentFixture<TrainingFormComponent>;
  let component: TrainingFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrainingFormComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        provideNoopAnimations(),
        provideNativeDateAdapter(),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({}) } } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TrainingFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('is invalid without an academy name', () => {
    component.form.patchValue({ academyName: '' });

    expect(component.form.valid).toBeFalse();
  });

  it('defaults self evaluation to a valid option and accepts a full valid submission', () => {
    component.form.patchValue({
      trainingDate: new Date(2026, 0, 10),
      academyName: 'Gracie Barra',
      selfEvaluation: 'Excellent',
      notes: ''
    });

    expect(component.form.valid).toBeTrue();
  });

  it('is invalid without a training date', () => {
    component.form.patchValue({ trainingDate: null, academyName: 'Gracie Barra' });

    expect(component.form.valid).toBeFalse();
  });
});
