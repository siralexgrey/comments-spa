import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http"
import { inject } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { catchError, throwError } from "rxjs"

export const errorInterceptor: HttpInterceptorFn  = (req, next) => {
  const snackbar = inject(MatSnackBar);
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      switch (err.status) {
        case 0:
          snackbar.open('No server connection', 'OK', { duration: 5000 });
          break;
        case 400:
          break
        default:
          snackbar.open('Something went wrong', 'OK', { duration: 5000 });
          break;
      }
      return throwError(() => err);
    }),
  );
}
