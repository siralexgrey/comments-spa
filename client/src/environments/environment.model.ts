import { HttpInterceptorFn } from "@angular/common/http";

export interface Environment {
  extraInterceptors: HttpInterceptorFn[];
}
