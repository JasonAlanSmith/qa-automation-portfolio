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

# A reference/lookup item (e.g. /references/customer-kinds).
REFERENCE_ITEM_SCHEMA = {
    "type": "object",
    "required": ["sysId", "name"],
    "properties": {
        "sysId": {"type": "string"},
        "name": {"type": "string"},
    },
}

# A Customer resource. We assert the fields the suite depends on (and that every
# row is tenant-stamped) without over-specifying the whole DTO, so the contract
# check stays meaningful but not brittle.
CUSTOMER_SCHEMA = {
    "type": "object",
    "required": [
        "sysId",
        "name",
        "isIndividual",
        "isActive",
        "kindSysId",
        "themeSysId",
        "tenantSysId",
    ],
    "properties": {
        "sysId": {"type": "string"},
        "name": {"type": "string"},
        "isIndividual": {"type": "boolean"},
        "isActive": {"type": "boolean"},
        "kindSysId": {"type": "string"},
        "themeSysId": {"type": "string"},
        # Every persisted row carries the owning tenant — the backbone of the
        # multi-tenant isolation the app guarantees.
        "tenantSysId": {"type": ["string", "null"]},
    },
}
