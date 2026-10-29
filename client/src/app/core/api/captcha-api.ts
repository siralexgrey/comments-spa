import { HttpClient } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { API_BASE_URL } from "./api-base-url";
import { CaptchaDto } from "../models/captcha.models";

@Service()
export class CaptchaApi {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE_URL);

  getCaptcha() {
    return this.http.get<CaptchaDto>(`${this.base}/captcha`);
  }
}
