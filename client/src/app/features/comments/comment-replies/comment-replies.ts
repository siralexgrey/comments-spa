import { Component, input, signal } from '@angular/core';
import { CommentDto } from '../../../core/models/comment.models';
import { DatePipe, I18nPluralPipe } from '@angular/common';
import { DATE_FORMAT } from '../../../shared/utils/date-format';
import { CommentAuthor } from '../comment-author/comment-author';
import { CommentReplyAction } from '../comment-reply-action/comment-reply-action';
import { CommentAttachment } from '../comment-attachment/comment-attachment';

@Component({
  imports: [
    DatePipe,
    CommentAuthor,
    CommentReplyAction,
    CommentAttachment,
    I18nPluralPipe
],
  selector: 'app-comment-replies',
  styleUrl: './comment-replies.scss',
  templateUrl: './comment-replies.html',
})
export class CommentReplies {
  readonly replies = input.required<CommentDto[]>();
  /** Nesting level of this list, 1 for direct replies to a root comment. Drives the rail colour. */
  readonly depth = input(1);

  protected readonly expanded = signal(false);
  protected readonly dateFormat = DATE_FORMAT;
  protected readonly replyMap = { one: '# reply', other: '# replies' };
}
