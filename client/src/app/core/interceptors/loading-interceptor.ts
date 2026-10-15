import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { LoadingState } from "../ui-state/loading-state";
import { finalize } from "rxjs";

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingState = inject(LoadingState);

  loadingState.start();
  return next(req).pipe(
    finalize(() => loadingState.stop())
  )
}
