import { Component, computed, inject, input } from '@angular/core';
import { CommentsStore } from '../data-access/comments-store';
import { CommentForm } from '../comment-form/comment-form';

@Component({
  imports: [CommentForm],
  selector: 'app-comment-reply-action',
  styleUrl: './comment-reply-action.scss',
  templateUrl: './comment-reply-action.html',
})
export class CommentReplyAction {
  protected readonly store = inject(CommentsStore);

  readonly commentId = input.required<number>();

  protected readonly isActive = computed(() => this.store.replyingToId() === this.commentId());

  toggle() {
    this.store.setReplyingToId(
      this.isActive() ? null : this.commentId()
    );
  }
}
