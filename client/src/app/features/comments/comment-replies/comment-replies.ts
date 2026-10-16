import { Component, input, signal } from '@angular/core';
import { CommentDto } from '../../../core/models/comment.models';
import { MatButton } from '@angular/material/button';
import { DatePipe } from '@angular/common';

@Component({
  imports: [
    MatButton,
    DatePipe
  ],
  selector: 'app-comment-replies',
  styleUrl: './comment-replies.scss',
  templateUrl: './comment-replies.html',
})
export class CommentReplies {
  readonly replies = input.required<CommentDto[]>();

  protected readonly expanded = signal(false);
}
