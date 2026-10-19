import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { isHttpUrl } from "./http-url.validator";

const ALLOWED_TAGS: Record<string, string[]> = {
  a: ['href', 'title'],
  code: [],
  i: [],
  strong: []
};

export const allowedHtmlValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value as string;
  if (!value) return null;

  const xml = `<root>${value}</root>`;
  const doc = new DOMParser().parseFromString(xml, 'application/xml');
  if (doc.querySelector('parsererror')) {
    return { invalidXhtml: true} ;
  }

  for (const element of doc.documentElement.querySelectorAll('*')) {
    const tag = element.tagName;
    if (!Object.hasOwn(ALLOWED_TAGS, tag)) {
      return { forbiddenTag: { tag } };
    }

    const allowedAttributes = ALLOWED_TAGS[tag];
    for (const attribute of element.attributes) {
      if (!allowedAttributes.includes(attribute.name)) {
        return  { forbiddenAttr: { tag, attr: attribute.name } };
      }
      if (attribute.name === 'href' && !isHttpUrl(attribute.value)) {
        return { invalidHref: true };
      }
    }
  }

  return null;
}
