import { Component, input, output } from '@angular/core';
import { CommentDto, DIR, GetCommentsQuery, SORT_BY, SortBy, SortDir } from '../../../core/models/comment.models';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { isInArray } from '../../../shared/utils/is-in-array';

@Component({
  imports: [
    MatTableModule,
    MatSortModule
  ],
  selector: 'app-comments-table',
  styleUrl: './comments-table.scss',
  templateUrl: './comments-table.html',
})
export class CommentsTable {
  readonly comments = input.required<CommentDto[]>();
  readonly sortBy = input.required<SortBy>();
  readonly sortDir = input.required<SortDir>();
  readonly sortChange = output<Pick<GetCommentsQuery, 'sortBy' | 'dir'>>();

  protected readonly columns = ['userName', 'email', 'createdAt'] as const;

  onSort(sort: Sort) {
    if(!isInArray(sort.active, SORT_BY) || !isInArray(sort.direction, DIR)) {
      return;
    }
    this.sortChange.emit({ sortBy: sort.active, dir: sort.direction });
  }
}
