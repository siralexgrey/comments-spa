import { Component, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatIconButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  imports: [MatIconButton, MatIconModule],
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => MarkupEditor), multi: true }
  ],
  selector: 'app-markup-editor',
  styleUrl: './markup-editor.scss',
  templateUrl: './markup-editor.html',
})
export class MarkupEditor implements ControlValueAccessor {
  protected readonly value = signal('');
  protected readonly disabled = signal(false);

  protected onChange: (value: string) => void = () => {};
  protected onTouched: () => void = () => {};

  protected handleInput(text: string): void {
    this.value.set(text);
    this.onChange(text);
  }

  writeValue(v: string | null): void {
    this.value.set(v ?? '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }
}
