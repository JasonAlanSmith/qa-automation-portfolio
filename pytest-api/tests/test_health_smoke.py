"""Smoke coverage: the core list endpoints answer and return the standard paged
envelope.

Demonstrated techniques:
- parameterization across many endpoints from one test body
- response schema validation of the shared paged-result contract
- using a session-scoped authenticated fixture for read-only checks
"""
from __future__ import annotations

import pytest
from jsonschema import validate

from src.schemas import PAGED_RESULT_SCHEMA

pytestmark = [pytest.mark.smoke, pytest.mark.regression]

# Paged list endpoints that an authenticated user should be able to read.
LIST_ENDPOINTS = [
    "/products",
    "/customers",
    "/user-stories",
    "/requirements",
    "/issues",
    "/tasks",
]


@pytest.mark.parametrize("endpoint", LIST_ENDPOINTS)
def test_list_endpoint_returns_paged_envelope(api, endpoint):
    response = api.get(endpoint, params={"pageSize": 5})

    assert response.status_code == 200, f"{endpoint} returned {response.status_code}"
    body = response.json()
    validate(instance=body, schema=PAGED_RESULT_SCHEMA)
    assert body["pageSize"] == 5
