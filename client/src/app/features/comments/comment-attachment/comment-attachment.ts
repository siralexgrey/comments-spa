import { Component, input } from '@angular/core';
import { AttachmentDto } from '../../../core/models/comment.models';
import { MatButton } from '@angular/material/button';

@Component({
  imports: [MatButton],
  selector: 'app-comment-attachment',
  styleUrl: './comment-attachment.scss',
  templateUrl: './comment-attachment.html',
})
export class CommentAttachment {
  readonly attachment = input.required<AttachmentDto>();
}
