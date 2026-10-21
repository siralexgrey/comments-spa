import { Directive, input } from '@angular/core';

@Directive({ selector: '[matCellDef]' })
export class TypedCellDef<T> {
  readonly matCellDefDataSource = input.required<T[]>();

  static ngTemplateContextGuard<T>(dir: TypedCellDef<T>, ctx: unknown): ctx is { $implicit: T } {
    return true;
  }
}
