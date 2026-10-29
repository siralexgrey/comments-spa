import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CommentsApi } from './comments-api';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { PagedResponse } from '../models/api.models';
import { CommentDto } from '../models/comment.models';
import { API_BASE_URL } from './api-base-url';

describe('CommentsApi', () => {
  let api: CommentsApi;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(CommentsApi);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('request comments with params', () => {
    let result: PagedResponse<CommentDto> | undefined;
    const response: PagedResponse<CommentDto> = { items: [], page: 1, pageSize: 25, totalCount: 0 };

    api.getComments({page: 3, sortBy: 'email', dir: 'asc'}).subscribe(r => (result = r));

    const req = httpTesting.expectOne(r => r.url === '/api/comments');
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('page')).toBe('3');
    expect(req.request.params.get('sortBy')).toBe('email');
    expect(req.request.params.get('dir')).toBe('asc');

    req.flush(response);
    expect(result).toEqual(response);
  });
})

describe('CommentsApi with custom API_BASE_URL', () => {
  let api: CommentsApi;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'http://test' },
      ],
    });
    api = TestBed.inject(CommentsApi);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('uses base url from token', () => {
    api.getComments({ page: 1, sortBy: 'createdAt', dir: 'desc' }).subscribe();
    httpTesting.expectOne(r => r.url === 'http://test/comments').flush({ items: [], page: 1, pageSize: 25, totalCount: 0 });
  });
});
