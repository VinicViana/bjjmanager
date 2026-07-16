import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RegisterComponent } from './register.component';

describe('RegisterComponent', () => {
  let fixture: ComponentFixture<RegisterComponent>;
  let component: RegisterComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
  });

  it('is invalid when the password is shorter than 6 characters', () => {
    component.form.setValue({ name: 'Vinicius', password: 'abc12' });

    expect(component.form.valid).toBeFalse();
    expect(component.form.controls.password.hasError('minlength')).toBeTrue();
  });

  it('is invalid when the name is empty', () => {
    component.form.setValue({ name: '', password: 'abcdef' });

    expect(component.form.valid).toBeFalse();
  });

  it('is valid with a name and a password of at least 6 characters', () => {
    component.form.setValue({ name: 'Vinicius', password: 'abcdef' });

    expect(component.form.valid).toBeTrue();
  });
});
