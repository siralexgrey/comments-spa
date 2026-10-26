# API CONTRACT

## Common Rules

- Base url: `/api` (except static `/uploads`)
- Format: JSON; exception - `POST /api/comments: multipart/form-data`; fields camelCase
- Dates: ISO 8601 string UTC ("2026-10-09T14:30:00Z")
- Errors format: see Errors

## Models (DTO)

### CommentDto

| Field | Type | Required | Note |
| -------- | -------- | -------- | -------- |
| id | number | yes | integer, unique |
| userName | string | yes | `^[a-zA-Z0-9]+$` |
| email | string | yes | |
| homePage | string \| null | yes | http/https |
| text | string | yes | sanitized XHTML, render through `[innerHTML]` |
| createdAt | string (ISO 8601) | yes | UTC |
| replies | CommentDto[] | yes | always array, can be empty, sorted by createdAt ASC |
| attachment | AttachmentDto \| null | yes | |

### CreateCommentDto

| Field | Type | Required | Note |
| -------- | -------- | -------- | -------- |
| parentId | number | no | integer, id of existing comment, not sent for root comment |
| userName | string | yes | 1-50 chars, `^[a-zA-Z0-9]+$` |
| email | string | yes | 1-254 chars |
| homePage | string | no | max 2048 chars; http/https; not sent if empty |
| text | string | yes | max 5000 chars, valid XHTML, only `<a href title>`, `<code>`, `<i>`, `<strong>` |
| attachment | File | no | image — JPG/GIF/PNG (if more than 320x240 - server resize proportionally, ≤ 5MB), text — TXT (≤ 100 KB); type check by content not extension |
| captchaId | string | yes | id from `GET /api/captcha` |
| captchaCode | string | yes | `^[a-zA-Z0-9]+$`; exactly 5 chars; one-time: invalid after check even if the code was wrong |

### AttachmentDto

| Field | Type | Required | Note |
| -------- | -------- | -------- | -------- |
| id | number | yes | integer, unique |
| originalName | string | yes | |
| type | 'text'\|'image' | yes | image — JPG/GIF/PNG, text — TXT |
| url | string | yes | relative path, e.g. `/uploads/<guid>.png` |
| width | number \| null | yes | in px, only for type="image", <= 320, null for type="text" |
| height | number \| null | yes | in px, only for type="image" <= 240, null for type="text" |

### PagedResponse\<T>

| Field | Type | Required | Note |
| -------- | -------- | -------- | -------- |
| items | T[] | yes | |
| page | number | yes | same as in request |
| pageSize | number | yes | |
| totalCount | number | yes | total items across all pages |

## Errors

### ProblemDetails

| Field | Type | Required | Note |
| -------- | -------- | -------- | -------- |
| type | string | no | default from ASP.NET |
| title | string | yes | |
| status | number | yes | see Status codes |
| errors | `{ [field: string]: string[] }` | no | key = request field name (multipart field or query param) |
| detail | string | no | detailed description of this error |

#### JSON example for incorrect captcha

```json
{
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
        "captchaCode": ["Invalid or expired captcha code"]
    }
}
```

## Status Codes

| Code | Name | When |
| -------- | -------- | -------- |
| 200 | OK | success |
| 201 | Created | post comment |
| 400 | Bad Request | invalid query params or body (validation, captcha, unknown parentId) |
| 404 | Not Found | invalid path of request or files, no ProblemDetails for files |
| 413 | Content Too Large | file too large; can be without ProblemDetails |
| 500 | Internal Server Error | all unexpected errors, ProblemDetails without errors, common error message |

## API

### GET /api/comments

- Query: page (number, ≥1, default 1), sortBy ('userName' | 'email' | 'createdAt', default 'createdAt'), dir ('asc' | 'desc', default 'desc')
- 200: PagedResponse\<CommentDto>
- 400: ProblemDetails — invalid sortBy (not from whitelist), dir (not from whitelist), page (< 1, not a number)
- Rules
  - pageSize: 25
  - default createdAt desc (LIFO);
  - sort and pagination only for root items, every root comes with full `replies` tree
  - `page` more than the last page -> 200 with `items: []`

#### JSON example

```json
{
    "items": [
        {
            "id": 1,
            "userName": "ExampleUser",
            "email": "example.user@gmail.com",
            "homePage": "https://example.com",
            "text": "Hello <strong>world</strong>, see <a href=\"https://example.com\" title=\"Example\">link</a>",
            "attachment": {
                "id": 1,
                "originalName": "example_image.png",
                "type": "image",
                "url": "/uploads/124u2323ohvsdv0932rucsju328ur.png",
                "width": 100,
                "height": 75
            },
            "createdAt": "2026-10-09T14:30:00Z",
            "replies": [
                {
                    "id": 2,
                    "userName": "AnotherUser",
                    "email": "another.example.user@gmail.com",
                    "homePage": "https://another_example.com",
                    "text": "Hello <strong>world</strong>, see <a href=\"https://example.com\" title=\"Example\">link</a>",
                    "attachment": {
                        "id": 2,
                        "originalName": "example_text.txt",
                        "type": "text",
                        "url": "/uploads/sdjvniuwr8923hc9293.txt",
                        "width": null,
                        "height": null
                    },
                    "createdAt": "2026-10-09T14:42:00Z",
                    "replies": []
                }
            ]
        }
    ],
    "page": 1,
    "pageSize": 25,
    "totalCount": 1
}
```

### POST /api/comments

- Body: CreateCommentDto
- 201: CommentDto
- 400: ProblemDetails - invalid multipart fields
- 413: File too large
- Rules:
  - without Location
  - new captcha after 400

### POST /api/comments/preview

- Body: `{ "text": "..." }`
- 200: `{ "html": "..." }`
- 400: ProblemDetails - errors.text
- Rules:
  - ProblemDetails errors return with error position

### GET /api/captcha

- Query: none
- 200: `{ id: string (GUID), imageDataUrl: string }`
- 500: ProblemDetails

### GET /uploads/{storedName}

- Query: none
- 200: File
- 404: No ProblemDetails
