import { HttpInterceptorFn, HttpResponse } from "@angular/common/http";
import { delay, of } from "rxjs";
import { createSeed } from "./mock-data";
import { getCommentsPage } from "./comments-mock";
import { SortBy, SortDir } from "../models/comment.models";
import { CaptchaDto } from "../models/captcha.models";

export const MOCK_CAPTCHA_CODE = 'ABC12';

const db = createSeed();
const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="120" height="40"><text x="10" y="28" font-size="24">${MOCK_CAPTCHA_CODE}</text></svg>`;
const imageDataUrl = 'data:image/svg+xml,' + encodeURIComponent(svg);

export const mockApiInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method === 'GET' && req.url === '/api/comments') {
    const query = {
      page: Number(req.params.get('page') || 1),
      sortBy: (req.params.get('sortBy') || 'createdAt') as SortBy,
      dir: (req.params.get('dir') || 'desc') as SortDir
    }
    return of(new HttpResponse({ status: 200, body: getCommentsPage(db, query) })).pipe(delay(400));
  }
  if (req.method === 'GET' && req.url === '/api/captcha') {
    return of(new HttpResponse({ status: 200, body: {id: '12223adsdf', imageDataUrl } satisfies CaptchaDto })).pipe(delay(400));
  }
  return next(req);
}
