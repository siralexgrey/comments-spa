import { Component, inject } from '@angular/core';
import { CommentsStore } from '../data-access/comments-store';
import { CommentsTable } from '../thread/comments-table/comments-table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { CommentForm } from '../form/comment-form/comment-form';

@Component({
  imports: [
    MatProgressBarModule,
    CommentsTable,
    CommentForm
],
  selector: 'app-comments-page',
  styleUrl: './comments-page.scss',
  templateUrl: './comments-page.html',
})
export class CommentsPage {
  protected readonly store = inject(CommentsStore);
}
