import { mockApiInterceptor } from "@core/mock/mock-api-interceptor";
import { Environment } from "./environment.model";

export const environment: Environment = {
  extraInterceptors: [mockApiInterceptor]
};
