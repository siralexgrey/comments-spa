import { afterNextRender, Component, ElementRef, forwardRef, inject, Injector, signal, viewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatIconButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { LinkDialog, LinkDialogResult } from '../link-dialog/link-dialog';
import { escapeXmlAttr } from '@shared/utils/escape-xml-attr';

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
  private readonly injector = inject(Injector);
  private readonly dialog = inject(MatDialog);

  protected readonly value = signal('');
  protected readonly disabled = signal(false);

  private readonly textareaElement = viewChild.required<ElementRef<HTMLTextAreaElement>>('textarea');

  protected onChange: (value: string) => void = () => {};
  protected onTouched: () => void = () => {};

  protected handleInput(text: string): void {
    this.value.set(text);
    this.onChange(text);
  }

  private insert(open: string, close: string): void {
    const ta = this.textareaElement().nativeElement;
    const { selectionStart: start, selectionEnd: end, value: text }  = ta;
    const newText = text.slice(0, start) + open + text.slice(start, end) + close + text.slice(end);
    this.handleInput(newText);
    afterNextRender(() => {
      ta.focus();
      ta.setSelectionRange(start + open.length, end + open.length);
    }, { injector: this.injector });
  }

  protected insertLink(): void {
    this.dialog.open(LinkDialog)
      .afterClosed()
      .subscribe(
        (result: LinkDialogResult | undefined) => {
          if(result) {
            const open = `<a href="${escapeXmlAttr(result.href)}"` + (result.title ? ` title="${escapeXmlAttr(result.title)}"` : ``) + `>`;
            const close = `</a>`;
            this.insert(open, close);
          }
        }
      )
  }

  protected wrap(tag: 'i' | 'strong' | 'code'): void {
    this.insert(`<${tag}>`, `</${tag}>`);
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
