import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { CommentDto, CreateCommentDto, GetCommentsQuery } from "../models/comment.models";
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

  createComment(body: CreateCommentDto) {
    const form = new FormData();
    form.append('userName', body.userName);
    form.append('email', body.email);
    form.append('text', body.text);
    form.append('captchaCode', body.captchaCode);
    form.append('captchaId', body.captchaId);
    if (body.attachment) {
      form.append('attachment', body.attachment);
    }
    if (body.homePage) {
      form.append('homePage', body.homePage);
    }
    if (body.parentId) {
      form.append('parentId', String(body.parentId));
    }
    return this.http.post<CommentDto>(`${this.base}/comments`, form);
  }
}
