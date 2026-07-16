import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { TechniqueFormComponent } from './technique-form.component';

describe('TechniqueFormComponent', () => {
  let fixture: ComponentFixture<TechniqueFormComponent>;
  let component: TechniqueFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TechniqueFormComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        provideNoopAnimations(),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({}) } } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TechniqueFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('starts with exactly one mandatory step', () => {
    expect(component.steps.length).toBe(1);
  });

  it('does not allow removing the last remaining step', () => {
    component.removeStep(0);

    expect(component.steps.length).toBe(1);
  });

  it('allows adding and then removing extra steps', () => {
    component.addStep();
    component.addStep();
    expect(component.steps.length).toBe(3);

    component.removeStep(1);
    expect(component.steps.length).toBe(2);
  });

  it('is invalid until name, position, description and at least one step description are filled in', () => {
    expect(component.form.valid).toBeFalse();

    component.form.patchValue({ name: 'Armbar', position: 'Mount', description: 'Classic armbar' });
    component.steps.at(0).setValue('Break posture');

    expect(component.form.valid).toBeTrue();
  });
});
