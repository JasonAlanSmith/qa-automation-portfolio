"""Shared fixtures.

Fixture *scope* is a deliberate design choice here:

- ``settings`` and ``api`` are **session-scoped** — we authenticate once for the
  whole run rather than per test. That's both faster and closer to how a real
  suite behaves.
- ``anon_client`` is **function-scoped** so authentication-flow tests get a clean,
  unauthenticated client they can log in/out of without disturbing the shared
  session.
"""
from __future__ import annotations

import pytest

from src.api_client import ApiClient
from src.config import settings as _settings


@pytest.fixture(scope="session")
def settings():
    return _settings


@pytest.fixture(scope="session")
def api(settings):
    """A session-scoped, authenticated client for read-only and setup calls."""
    client = ApiClient(settings)
    response = client.login_as_test_user()
    assert response.status_code == 200, (
        f"Test-user login failed ({response.status_code}). "
        f"Is MaelstromOps running at {settings.base_url}? Body: {response.text}"
    )
    yield client
    client.logout()
    client.close()


@pytest.fixture()
def anon_client(settings):
    """A fresh, unauthenticated client (for login/logout/refresh-flow tests)."""
    with ApiClient(settings) as client:
        yield client
