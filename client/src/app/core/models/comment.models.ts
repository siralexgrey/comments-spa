export interface CommentDto {
  id: number,
  userName: string,
  email: string,
  homePage: string | null,
  text: string,
  createdAt: string,
  replies: CommentDto[],
  attachment: AttachmentDto | null
}

export interface CreateCommentDto {
  parentId?: number,
  userName: string,
  email: string,
  homePage?: string,
  text: string,
  attachment?: File,
  captchaId: string,
  captchaCode: string
}

export interface GetCommentsQuery {
  page: number,
  sortBy: SortBy,
  dir: SortDir
}

export interface AttachmentDto {
  id: number,
  originalName: string,
  type: AttachmentType,
  url: string,
  width: number | null,
  height: number | null
}

export type AttachmentType = 'text' | 'image'
export const SORT_BY = ['createdAt', 'userName', 'email'] as const;
export type SortBy = (typeof SORT_BY)[number];
export const DIR = ['asc', 'desc'] as const;
export type SortDir = (typeof DIR)[number];
