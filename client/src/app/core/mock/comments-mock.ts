import { PagedResponse } from "../models/api.models";
import { CommentDto, GetCommentsQuery} from "../models/comment.models";

const PAGE_SIZE = 25;

export function getCommentsPage(all: CommentDto[], query: GetCommentsQuery): PagedResponse<CommentDto> {
  const dir = query.dir === 'asc' ? 1 : -1;
  const sorted = all.toSorted((a, b) => a[query.sortBy].localeCompare(b[query.sortBy]) * dir);
  const startIndex = (query.page - 1) * PAGE_SIZE;
  const items = sorted.slice(startIndex, startIndex + PAGE_SIZE);

  return {
    items,
    page: query.page,
    pageSize: PAGE_SIZE,
    totalCount: all.length
  }
}
