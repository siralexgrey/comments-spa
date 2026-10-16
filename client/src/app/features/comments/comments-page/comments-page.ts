import { Component, inject } from '@angular/core';
import { CommentsStore } from '../data-access/comments-store';
import { MatButton } from '@angular/material/button';
import { CommentsTable } from '../comments-table/comments-table';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  imports: [
    MatButton,
    MatProgressBarModule,
    CommentsTable,
  ],
  selector: 'app-comments-page',
  styleUrl: './comments-page.scss',
  templateUrl: './comments-page.html',
})
export class CommentsPage {
  protected readonly store = inject(CommentsStore);
}
