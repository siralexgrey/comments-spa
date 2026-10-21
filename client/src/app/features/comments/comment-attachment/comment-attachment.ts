import { Component, inject, input } from '@angular/core';
import { AttachmentDto } from '../../../core/models/comment.models';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { AttachmentDialog } from '../attachment-dialog/attachment-dialog';

@Component({
  imports: [MatButton, MatIcon],
  selector: 'app-comment-attachment',
  styleUrl: './comment-attachment.scss',
  templateUrl: './comment-attachment.html',
})
export class CommentAttachment {
  private readonly dialog = inject(MatDialog);

  readonly attachment = input.required<AttachmentDto>();

  protected openDialog() {
    this.dialog.open<AttachmentDialog, AttachmentDto>(AttachmentDialog, { data: this.attachment() });
  }
}
