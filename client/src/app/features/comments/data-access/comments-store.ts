import { computed, inject, linkedSignal, Service, signal } from "@angular/core";
import { CommentDto, GetCommentsQuery, SortBy, SortDir } from "../../../core/models/comment.models";
import { CommentsApi } from "../../../core/api/comments-api";
import { rxResource } from "@angular/core/rxjs-interop";
import { PagedResponse } from "../../../core/models/api.models";

@Service()
export class CommentsStore {
  private readonly api = inject(CommentsApi);

  private readonly _query = signal<GetCommentsQuery>({ page: 1, sortBy: 'createdAt', dir: 'desc' });
  readonly query = this._query.asReadonly();

  private readonly comments = rxResource({
    params: () => this._query(),
    stream: ({ params }) => this.api.getComments(params),
  });

  private readonly lastResponse = linkedSignal<PagedResponse<CommentDto> | undefined, PagedResponse<CommentDto> | undefined>({
    source: () => this.comments.hasValue() ? this.comments.value() : undefined,
    computation: (next, previous) => next ?? previous?.value
  });

  readonly items = computed(() => this.lastResponse()?.items ?? []);
  readonly totalCount = computed(() => this.lastResponse()?.totalCount ?? 0);
  readonly pageSize = computed(() => this.lastResponse()?.pageSize ?? 25);
  readonly isLoading = this.comments.isLoading;
  readonly error = this.comments.error;

  setPage(page: number) {
    this._query.update(q => ({ ...q, page }));
  }

  setSort(sortBy: SortBy, dir: SortDir) {
    this._query.update(q => ({ ...q, sortBy, dir, page: 1 }));
  }
}
