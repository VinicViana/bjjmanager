import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
  },
  {
    path: 'trainings',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/trainings/training-list.component').then((m) => m.TrainingListComponent)
  },
  {
    path: 'trainings/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/trainings/training-form.component').then((m) => m.TrainingFormComponent)
  },
  {
    path: 'trainings/:id/edit',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/trainings/training-form.component').then((m) => m.TrainingFormComponent)
  },
  {
    path: 'techniques',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/techniques/technique-list.component').then((m) => m.TechniqueListComponent)
  },
  {
    path: 'techniques/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/techniques/technique-form.component').then((m) => m.TechniqueFormComponent)
  },
  {
    path: 'techniques/:id/edit',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/techniques/technique-form.component').then((m) => m.TechniqueFormComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
