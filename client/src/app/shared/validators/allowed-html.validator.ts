import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

const ALLOWED_TAGS = ['a', 'code', 'i', 'strong'];

export const allowedHtmlValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value as string;
  if (!value) return null;

  const xml = `<root>${value}</root>`;
  const doc = new DOMParser().parseFromString(xml, 'application/xml');
  if (doc.querySelector('parsererror')) {
    return { invalidXhtml: true} ;
  }

  for (const element of doc.documentElement.querySelectorAll('*')) {
    if (!ALLOWED_TAGS.includes(element.tagName)) {
      return { forbiddenTag: { tag: element.tagName } };
    }
  }

  return null;
}
