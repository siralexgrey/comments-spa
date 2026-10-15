import { computed, inject, Service, signal } from "@angular/core";
import { GetCommentsQuery, SortBy, SortDir } from "../../../core/models/comment.models";
import { CommentsApi } from "../../../core/api/comments-api";
import { rxResource } from "@angular/core/rxjs-interop";

@Service()
export class CommentsStore {
  private readonly api = inject(CommentsApi);

  private readonly _query = signal<GetCommentsQuery>({ page: 1, sortBy: 'createdAt', dir: 'desc' });
  readonly query = this._query.asReadonly();

  private readonly comments = rxResource({
    params: () => this._query(),
    stream: ({ params }) => this.api.getComments(params),
  });

  readonly items = computed(() => (this.comments.hasValue() ? this.comments.value().items : []));
  readonly totalCount = computed(() => (this.comments.hasValue() ? this.comments.value().totalCount : 0));
  readonly isLoading = this.comments.isLoading;
  readonly error = this.comments.error;

  setPage(page: number) {
    this._query.update(q => ({ ...q, page }));
  }

  setSort(sortBy: SortBy, dir: SortDir) {
    this._query.update(q => ({ ...q, sortBy, dir, page: 1 }));
  }
}
