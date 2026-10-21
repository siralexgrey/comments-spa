import { HttpClient } from "@angular/common/http";
import { inject, Service } from "@angular/core";

@Service()
export class AttachmentApi {
  private readonly http = inject(HttpClient);

  getText(url: string) {
    return this.http.get(url, { responseType: 'text' });
  }
}
