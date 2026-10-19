import { Component, input } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-comment-author',
  templateUrl: './comment-author.html',
})
export class CommentAuthor {
  readonly userName = input.required<string>();
  readonly homePage = input<string | null>(null);
}
