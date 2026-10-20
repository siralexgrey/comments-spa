import { Component, ElementRef, inject, input, output, signal, viewChild } from '@angular/core';
import { CommentDto, CreateCommentDto } from '../../../core/models/comment.models';
import { FormGroupDirective, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButton, MatIconButton } from '@angular/material/button';
import { httpUrlValidator } from '../../../shared/validators/http-url.validator';
import { allowedHtmlValidator } from '../../../shared/validators/allowed-html.validator';
import { CaptchaApi } from '../../../core/api/captcha-api';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatIconModule } from '@angular/material/icon';
import { CommentsStore } from '../data-access/comments-store';
import { MarkupEditor } from '../markup-editor/markup-editor';
import { ErrorStateMatcher } from '@angular/material/core';
import { CommentsApi } from '../../../core/api/comments-api';
import { map } from 'rxjs';
import { PreviewResponse } from '../../../core/models/preview.models';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { attachmentValidator } from '../../../shared/validators/attachment.validator';
import { DecimalPipe } from '@angular/common';

@Component({
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButton,
    MatIconButton,
    MatIconModule,
    MarkupEditor,
    MatProgressBarModule,
    DecimalPipe
  ],
  selector: 'app-comment-form',
  styleUrl: './comment-form.scss',
  templateUrl: './comment-form.html',
})
export class CommentForm {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly commentsApi = inject(CommentsApi);
  private readonly captchaApi = inject(CaptchaApi);
  private readonly store = inject(CommentsStore);

  protected readonly errorStateMatcher = inject(ErrorStateMatcher);

  protected readonly formDir = viewChild.required(FormGroupDirective);
  private readonly fileInput = viewChild.required<ElementRef<HTMLInputElement>>('fileInput')

  readonly parentId = input<number>();
  readonly submitted = output<CommentDto>();

  private readonly previewText = signal<string | undefined>(undefined);

  readonly captcha = rxResource({
    stream: () => this.captchaApi.getCaptcha()
  })

  protected readonly preview = rxResource({
    params: () => this.previewText(),
    stream: ({ params }) => this.commentsApi.preview(params).pipe(map((value: PreviewResponse) => value.html))
  })

  form = this.fb.group({
    userName:['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9]+$/)]],
    email:['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    homePage:['', [ Validators.maxLength(2048), httpUrlValidator]],
    text: ['', [Validators.required, Validators.maxLength(5000), allowedHtmlValidator]],
    captchaCode: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9]+$/)]],
    attachment: this.fb.control<File | null>(null, attachmentValidator)
  });

  protected submit() {
    const captcha = this.captcha.value();
    if (!captcha) {
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const formValue = this.form.getRawValue();

    const createCommentDto: CreateCommentDto = {
      userName: formValue.userName,
      email: formValue.email,
      text: formValue.text,
      captchaCode: formValue.captchaCode,
      captchaId: captcha.id
    }

    if (formValue.homePage.length) {
      createCommentDto.homePage = formValue.homePage;
    }
    if (this.parentId()) {
      createCommentDto.parentId = this.parentId();
    }
    if (formValue.attachment) {
      createCommentDto.attachment = formValue.attachment;
    }
    this.store.createComment(createCommentDto)
      .subscribe({
        next: (value) => {
          this.formDir().resetForm();
          this.fileInput().nativeElement.value = '';
          this.captcha.reload();
          this.previewText.set(undefined);
          this.submitted.emit(value);
        },
        error: () => {
          this.captcha.reload();
          this.form.controls.captchaCode.reset();
        },
      })
  }

  protected showPreview() {
    if (this.form.controls.text.invalid) {
      this.form.controls.text.markAsTouched();
      this.previewText.set(undefined);
      return;
    }

    this.previewText.set(this.form.controls.text.getRawValue());
  }

  protected pick(files: FileList | null) {
    const file = files?.item(0) ?? null;
    this.form.controls.attachment.setValue(file);
    this.form.controls.attachment.markAsTouched();
  }
}
