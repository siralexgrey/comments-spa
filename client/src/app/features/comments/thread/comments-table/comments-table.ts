import { Component, input, output, TrackByFunction } from '@angular/core';
import { CommentDto, DIR, GetCommentsQuery, SORT_BY, SortBy, SortDir } from '@core/models/comment.models';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { isInArray } from '@shared/utils/is-in-array';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { DatePipe } from '@angular/common';
import { CommentReplies } from '../comment-replies/comment-replies';
import { DATE_FORMAT } from '@shared/utils/date-format';
import { CommentAuthor } from '../comment-author/comment-author';
import { CommentReplyAction } from '../comment-reply-action/comment-reply-action';
import { CommentAttachment } from '../../attachment/comment-attachment/comment-attachment';
import { TypedCellDef } from '@shared/directives/typed-cell-def';

@Component({
  imports: [
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    DatePipe,
    CommentReplies,
    CommentAuthor,
    CommentReplyAction,
    CommentAttachment,
    TypedCellDef
],
  selector: 'app-comments-table',
  styleUrl: './comments-table.scss',
  templateUrl: './comments-table.html',
})
export class CommentsTable {
  readonly comments = input.required<CommentDto[]>();
  readonly sortBy = input.required<SortBy>();
  readonly sortDir = input.required<SortDir>();
  readonly page = input.required<number>();
  readonly totalCount = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly sortChange = output<Pick<GetCommentsQuery, 'sortBy' | 'dir'>>();
  readonly pageChange = output<number>();

  protected readonly columns = ['userName', 'email', 'createdAt'] as const;
  protected readonly dateFormat = DATE_FORMAT;

  protected readonly trackById: TrackByFunction<CommentDto> = (_, item: CommentDto): number => {
    return item.id;
  }

  onSort(sort: Sort) {
    if(!isInArray(sort.active, SORT_BY) || !isInArray(sort.direction, DIR)) {
      return;
    }
    this.sortChange.emit({ sortBy: sort.active, dir: sort.direction });
  }

  onPageChange(pageEvent: PageEvent) {
    this.pageChange.emit(pageEvent.pageIndex + 1)
  }
}
