import { CommentDto } from '../models/comment.models';
import { getCommentsPage } from './comments-mock';

function makeComment(overrides: Partial<CommentDto> = {}) : CommentDto {
  return {
    id: 1,
    userName: 'Aby',
    createdAt: '2026-10-09T14:30:00Z',
    email: 'a@b.com',
    homePage: null,
    text: 'some text',
    attachment: null,
    replies: [],
    ...overrides
  }
}

describe('getCommentsPage', () => {
  let data: CommentDto[];
  let manyPagesData: CommentDto[];
  beforeEach(() => {
    data = [
      makeComment({ userName: 'Hanna', createdAt: '2026-10-03T12:00:00Z'}),
      makeComment({ userName: 'Dmytro', createdAt: '2026-10-01T10:00:00Z'}),
      makeComment({ userName: 'Volodymyr', createdAt: '2026-10-02T14:00:00Z'})
    ];
    manyPagesData = Array.from({ length: 60}, (_, i) => makeComment({id: i + 1}));
  })

  it('sorted by userName ascending', () => {
    // Act
    const result = getCommentsPage(data, {page: 1, sortBy: 'userName', dir: 'asc'});
    //Assert
    expect(result.items.map(c => c.userName)).toEqual(['Dmytro', 'Hanna', 'Volodymyr']);
  });

  it('sorted by userName desc', () => {
    // Act
    const result = getCommentsPage(data, {page: 1, sortBy: 'userName', dir: 'desc'});
    //Assert
    expect(result.items.map(c => c.userName)).toEqual(['Volodymyr', 'Hanna', 'Dmytro']);
  });

  it('sorted by createdAt asc', () => {
    // Act
    const result = getCommentsPage(data, {page: 1, sortBy: 'createdAt', dir: 'asc'});
    //Assert
    expect(result.items.map(c => c.userName)).toEqual(['Dmytro', 'Volodymyr', 'Hanna']);
  });

  it('sorted by createdAt desc', () => {
    // Act
    const result = getCommentsPage(data, {page: 1, sortBy: 'createdAt', dir: 'desc'});
    //Assert
    expect(result.items.map(c => c.userName)).toEqual(['Hanna', 'Volodymyr', 'Dmytro']);
  });

  it('page 2 out of 60 items', () => {
    // Act
    const result = getCommentsPage(manyPagesData, {page: 2, sortBy: 'createdAt', dir: 'asc'});
    //Assert
    expect(result.items).toHaveLength(25);
    expect(result.items[0].id).toBe(26);
    expect(result.items.at(-1)?.id).toBe(50);
    expect(result.totalCount).toBe(60);
  });

  it('page 3 out of 60 items', () => {
    // Act
    const result = getCommentsPage(manyPagesData, {page: 3, sortBy: 'createdAt', dir: 'asc'});
    //Assert
    expect(result.items).toHaveLength(10);
    expect(result.items[0].id).toBe(51);
    expect(result.items.at(-1)?.id).toBe(60);
    expect(result.totalCount).toBe(60);
  });

  it('page out of bounds', () => {
    // Act
    const result = getCommentsPage(manyPagesData, {page: 999, sortBy: 'createdAt', dir: 'asc'});
    //Assert
    expect(result.items).toHaveLength(0);
    expect(result.totalCount).toBe(60);
  });

  it('doesn\'t mutate input array', () => {
    // Arrange
    const userNamesCopy = data.map(c => c.userName);
    // Act
    getCommentsPage(data, {page: 1, sortBy: 'userName', dir: 'asc'});
    //Assert
    expect(data.map(c => c.userName)).toEqual(userNamesCopy);
  });
})
