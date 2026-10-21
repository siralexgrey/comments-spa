import { AttachmentDto, CommentDto } from '../models/comment.models';

const ROOT_COUNT = 60;
const BASE_TIME = Date.parse('2026-10-09T12:00:00Z');
const HOUR = 60 * 60 * 1000;

const NAMES = ['Hanna', 'Dmytro', 'Volodymyr', 'Olena', 'Taras', 'Iryna', 'Bohdan', 'Sofiia'];

const TEXTS = [
  'Great article, <strong>thanks</strong>!',
  'Check the docs: <a href="https://angular.dev" title="Angular">angular.dev</a>',
  'Use <code>toSorted()</code> instead of <code>sort()</code>.',
  'I <i>partly</i> agree with this.',
  'Plain text comment without tags.',
];

const IMAGE: AttachmentDto = {
  id: 1,
  originalName: 'sample.png',
  type: 'image',
  url: '/mock-uploads/sample.png',
  width: 320,
  height: 240,
};

const TEXT_FILE: AttachmentDto = {
  id: 2,
  originalName: 'notes.txt',
  type: 'text',
  url: '/mock-uploads/sample.txt',
  width: null,
  height: null,
};

/** ISO date `hoursAgo` hours before BASE_TIME — deterministic, no Date.now(). */
function isoHoursAgo(hoursAgo: number): string {
  return new Date(BASE_TIME - hoursAgo * HOUR).toISOString();
}

function makeRoot(i: number): CommentDto {
  const name = NAMES[i % NAMES.length];
  return {
    id: i + 1,
    userName: `${name}${i + 1}`,
    email: `${name.toLowerCase()}${i + 1}@example.com`,
    homePage: i % 4 === 0 ? `https://${name.toLowerCase()}.example.com` : null,
    text: TEXTS[i % TEXTS.length],
    createdAt: isoHoursAgo(ROOT_COUNT - i), // id 60 is the newest
    replies: [],
    attachment: i % 10 === 3 ? IMAGE : i % 10 === 7 ? TEXT_FILE : null,
  };
}

let replyId = 1000;

function reply(
  userName: string,
  text: string,
  hoursAgo: number,
  replies: CommentDto[] = [],
): CommentDto {
  replyId += 1;
  return {
    id: replyId,
    userName,
    email: `${userName.toLowerCase()}@example.com`,
    homePage: null,
    text,
    createdAt: isoHoursAgo(hoursAgo),
    replies,
    attachment: null,
  };
}

export function createSeed(): CommentDto[] {
  replyId = 1000;
  const roots = Array.from({ length: ROOT_COUNT }, (_, i) => makeRoot(i));

  // Newest root: 4 levels deep.
  roots[59].replies = [
    reply('Olena', 'First reply', 0.9, [
      reply('Taras', 'Reply to <i>Olena</i>', 0.7, [
        reply('Iryna', 'Third level', 0.5, [reply('Bohdan', 'Fourth level, deepest', 0.3)]),
      ]),
    ]),
    reply('Sofiia', 'Second reply on the same level', 0.8),
  ];

  // Root on page 1: two short branches.
  roots[55].replies = [
    reply('Dmytro', 'Agree, <strong>+1</strong>', 4.5, [reply('Hanna', 'Me too', 4.2)]),
    reply('Volodymyr', 'Not sure about this', 4.4),
  ];

  // Root on page 2: one reply, so nesting is visible after paging too.
  roots[30].replies = [reply('Taras', 'Late reply on an older comment', 20)];

  return roots;
}
