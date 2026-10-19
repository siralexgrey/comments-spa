import { Component, input, signal } from '@angular/core';
import { CommentDto } from '../../../core/models/comment.models';
import { DatePipe } from '@angular/common';
import { DATE_FORMAT } from '../../../shared/utils/date-format';
import { CommentAuthor } from '../comment-author/comment-author';

@Component({
  imports: [
    DatePipe,
    CommentAuthor
],
  selector: 'app-comment-replies',
  templateUrl: './comment-replies.html',
})
export class CommentReplies {
  readonly replies = input.required<CommentDto[]>();

  protected readonly expanded = signal(false);
  protected readonly dateFormat = DATE_FORMAT;
}
