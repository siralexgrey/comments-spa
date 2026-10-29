import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { AttachmentDto } from '@core/models/comment.models';
import { rxResource } from '@angular/core/rxjs-interop';
import { AttachmentApi } from '@core/api/attachment-api';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';

@Component({
  imports: [
    MatDialogModule,
    MatProgressBarModule,
    MatButtonModule
  ],
  selector: 'app-attachment-dialog',
  styleUrl: './attachment-dialog.scss',
  templateUrl: './attachment-dialog.html',
})
export class AttachmentDialog {
  private readonly attachmentApi = inject(AttachmentApi);
  protected readonly data = inject<AttachmentDto>(MAT_DIALOG_DATA);

  protected readonly text = rxResource({
    params: () => this.data.type === 'text' ? this.data.url : undefined,
    stream: ({ params }) => this.attachmentApi.getText(params)
  })
}
