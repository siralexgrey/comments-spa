import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { httpUrlValidator } from '../../../shared/validators/http-url.validator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButton } from '@angular/material/button';
import { MatInput } from '@angular/material/input';

export interface LinkDialogResult {
  href: string,
  title?: string
}

@Component({
  imports: [MatDialogModule, MatFormFieldModule, ReactiveFormsModule, MatButton, MatInput],
  selector: 'app-link-dialog',
  styleUrl: './link-dialog.scss',
  templateUrl: './link-dialog.html',
})
export class LinkDialog {
  private readonly ref = inject(MatDialogRef<LinkDialog, LinkDialogResult>);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly form = this.fb.group({
    href: ['', [Validators.required, httpUrlValidator]],
    title: ['']
  });

  protected confirm(): void {
    const obj: LinkDialogResult = {
      href: this.form.controls.href.getRawValue()
    };
    const title = this.form.controls.title.getRawValue();
    if (title.length) {
      obj.title = title;
    }
    this.ref.close(obj);
  }
}
