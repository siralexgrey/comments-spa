import { Component, inject, input, output } from '@angular/core';
import { CreateCommentDto } from '../../../core/models/comment.models';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { httpUrlValidator } from '../../../shared/validators/http-url.validator';
import { allowedHtmlValidator } from '../../../shared/validators/allowed-html.validator';

@Component({
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButton],
  selector: 'app-comment-form',
  styleUrl: './comment-form.scss',
  templateUrl: './comment-form.html',
})
export class CommentForm {
  private fb = inject(NonNullableFormBuilder);

  readonly parentId = input<number>();
  readonly submitted = output<CreateCommentDto>();

  form = this.fb.group({
    userName:['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9]+$/)]],
    email:['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    homePage:['', [ Validators.maxLength(2048), httpUrlValidator]],
    text: ['', [Validators.required, Validators.maxLength(5000), allowedHtmlValidator]],
    captchaCode: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9]+$/)]],
  });

  submit() {
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
      captchaId: '123ABC'
    }

    if (formValue.homePage.length) {
      createCommentDto.homePage = formValue.homePage;
    }
    if (this.parentId()) {
      createCommentDto.parentId = this.parentId();
    }
    this.submitted.emit(createCommentDto);
  }
}
