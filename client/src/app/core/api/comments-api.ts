import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { CommentDto, GetCommentsQuery } from "../models/comment.models";
import { PagedResponse } from "../models/api.models";
import { API_BASE_URL } from "./api-base-url";

@Service()
export class CommentsApi {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE_URL);

  getComments(query: GetCommentsQuery) {
    const params = new HttpParams()
      .set('page', query.page)
      .set('sortBy', query.sortBy)
      .set('dir', query.dir);
    return this.http.get<PagedResponse<CommentDto>>(`${this.base}/comments`, { params });
  }
}
