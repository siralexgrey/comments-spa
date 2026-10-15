import { computed, Service, signal } from "@angular/core";

@Service()
export class LoadingState {
  private readonly counter = signal(0);

  readonly isLoading = computed(() => this.counter() > 0);

  start() {
    this.counter.update(v => v + 1);
  }

  stop() {
    this.counter.update(v => v - 1);
  }
}
