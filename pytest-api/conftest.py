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
from src.factories import customer_payload


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


@pytest.fixture(scope="session")
def reference_ids(api):
    """Resolve real reference FKs (a customer kind + theme) once per run.

    Dependencies that CRUD payloads need are fetched from the API at runtime
    rather than hard-coded — so the suite stays valid across environments and
    re-seeds.
    """
    kinds = api.get("/references/customer-kinds").json()
    themes = api.get("/references/customer-themes").json()
    assert kinds and themes, "customer kind/theme reference data must be seeded"
    return {"kind": kinds[0]["sysId"], "theme": themes[0]["sysId"]}


@pytest.fixture()
def customer_factory(api, reference_ids):
    """Create customers on demand and guarantee they're cleaned up.

    The classic factory-as-fixture: a test calls ``customer_factory(name=...)`` to
    create as many valid customers as it needs; every one is tracked and deleted in
    teardown — even if the test fails mid-way — so tests stay independent and leave
    no residue. Returns the created resource (dict, incl. ``sysId``).
    """
    created_ids: list[str] = []

    def _create(**overrides):
        payload = customer_payload(reference_ids["kind"], reference_ids["theme"], **overrides)
        response = api.post("/customers", json=payload)
        assert response.status_code == 201, (
            f"customer_factory failed to create ({response.status_code}): {response.text}"
        )
        body = response.json()
        created_ids.append(body["sysId"])
        return body

    yield _create

    # Teardown: delete everything we made. A 404 (a test already deleted it) is fine.
    for sysid in created_ids:
        api.delete(f"/customers/{sysid}")


@pytest.fixture()
def second_tenant_client(settings):
    """An authenticated client for a *different* tenant, or skip if not configured.

    The cross-tenant isolation test needs two real accounts in two orgs. Rather
    than fake it, we skip cleanly unless MOPS_TEST_EMAIL_2 / _PASSWORD_2 are set.
    """
    if not settings.has_second_tenant:
        pytest.skip("second tenant not configured (set MOPS_TEST_EMAIL_2 / _PASSWORD_2)")
    client = ApiClient(settings)
    response = client.login(settings.test_email_2, settings.test_password_2)
    assert response.status_code == 200, (
        f"second-tenant login failed ({response.status_code}): {response.text}"
    )
    yield client
    client.logout()
    client.close()
