import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

const ALLOWED_PROTOCOL = ['http:', 'https:'];

export const httpUrlValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value as string;
  if (!value) return null;

  try {
    const url = new URL(value);
    return ALLOWED_PROTOCOL.includes(url.protocol) ? null : { httpUrl: true };
  }
  catch {
    return { httpUrl: true };
  }
}
