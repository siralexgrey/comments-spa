import { HttpErrorResponse, HttpInterceptorFn, HttpResponse } from "@angular/common/http";
import { delay, of, throwError } from "rxjs";
import { createSeed } from "./mock-data";
import { getCommentsPage } from "./comments-mock";
import { DIR, SORT_BY, SortBy, SortDir } from "../models/comment.models";
import { CaptchaDto } from "../models/captcha.models";
import { isInArray } from '../../shared/utils/is-in-array';
import { ProblemDetails } from "../models/api.models";

export const MOCK_CAPTCHA_CODE = 'ABC12';

const db = createSeed();
const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="120" height="40"><text x="10" y="28" font-size="24">${MOCK_CAPTCHA_CODE}</text></svg>`;
const imageDataUrl = 'data:image/svg+xml,' + encodeURIComponent(svg);

export const mockApiInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method === 'GET' && req.url === '/api/comments') {
    const errors: Record<string, string[]> = {};
    const page = Number(req.params.get('page') || 1);
    const sortBy = req.params.get('sortBy') ?? 'createdAt';
    const dir = req.params.get('dir') ?? 'desc';
    if (!Number.isInteger(page) || page < 1) {
      errors['page'] = ['Page must be an integer ≥ 1'];
    }
    if(!isInArray(sortBy, SORT_BY)) {
      errors['sortBy'] = ['Invalid sortBy'];
    }
    if(!isInArray(dir, DIR)) {
      errors['dir'] = ['Invalid dir'];
    }
    if (Object.keys(errors).length > 0) {
      return throwError(() => new HttpErrorResponse({
        status: 400,
        error: {
          title: 'One or more validation errors occurred.',
          status: 400,
          errors
        } satisfies ProblemDetails,
        url: req.url
      }));
    }
    const query = {page, sortBy: sortBy as SortBy, dir: dir as SortDir};
    return of(new HttpResponse({ status: 200, body: getCommentsPage(db, query) })).pipe(delay(400));
  }
  if (req.method === 'GET' && req.url === '/api/captcha') {
    return of(new HttpResponse({ status: 200, body: {id: '12223adsdf', imageDataUrl } satisfies CaptchaDto })).pipe(delay(400));
  }
  return next(req);
}
