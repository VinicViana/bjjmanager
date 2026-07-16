import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status !== 401) {
        snackBar.open(extractErrorMessage(error), 'Close', { duration: 5000 });
      }

      return throwError(() => error);
    })
  );
};

function extractErrorMessage(error: HttpErrorResponse): string {
  const body = error.error as { errors?: Record<string, string[]>; title?: string } | null;

  if (body?.errors) {
    return Object.values(body.errors).flat().join(' ');
  }

  if (body?.title) {
    return body.title;
  }

  return 'Something went wrong. Please try again.';
}
