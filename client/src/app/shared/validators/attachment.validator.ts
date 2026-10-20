import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

const MAX_SIZE_BY_TYPE: Record<string, number> = {
  'image/jpeg': 5 * 1024 * 1024,
  'image/gif': 5 * 1024 * 1024,
  'image/png': 5 * 1024 * 1024,
  'text/plain': 100 * 1024
};

export const attachmentValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const file = control.value;
  if (!(file instanceof File)) return null;
  if (!Object.hasOwn(MAX_SIZE_BY_TYPE, file.type)) {
    return { forbiddenType: { type: file.type } };
  }
  if (file.size > MAX_SIZE_BY_TYPE[file.type]) {
    return { sizeExceeded: { type: file.type, actualSize: file.size, allowedSize: MAX_SIZE_BY_TYPE[file.type] }};
  }
  return null;
}
