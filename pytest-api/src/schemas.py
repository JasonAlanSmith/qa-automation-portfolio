"""jsonschema definitions for validating API response *shape* (contract checks).

Asserting on structure (not just status codes) catches silent contract drift — a
renamed or dropped field fails the test even when the HTTP call "succeeds".
"""

# The standard paged-list envelope returned by every list endpoint.
PAGED_RESULT_SCHEMA = {
    "type": "object",
    "required": ["items", "page", "pageSize", "totalCount", "totalPages"],
    "properties": {
        "items": {"type": "array"},
        "page": {"type": "integer", "minimum": 1},
        "pageSize": {"type": "integer", "minimum": 1},
        "totalCount": {"type": "integer", "minimum": 0},
        "totalPages": {"type": "integer", "minimum": 0},
    },
}

# The identity payload from /authentication/me.
ME_SCHEMA = {
    "type": "object",
    "required": ["email", "tenant", "user", "roles"],
    "properties": {
        "email": {"type": "string"},
        "tenant": {"type": ["string", "null"]},
        "user": {"type": ["string", "null"]},
        "roles": {"type": "array", "items": {"type": "string"}},
    },
}
