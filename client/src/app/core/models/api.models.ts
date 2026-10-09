export interface PagedResponse<T> {
  items: T[],
  page: number,
  pageSize: number,
  totalCount: number
}

export interface ProblemDetails {
  type?: string,
  title: string,
  status: number,
  errors?: Record<string, string[]>,
  detail?: string
}
