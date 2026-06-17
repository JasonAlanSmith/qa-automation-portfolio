"""Authentication & session behaviour.

Demonstrated techniques:
- negative testing and parameterized invalid-credential cases
- response **schema** validation (not just status codes)
- the full session lifecycle: login -> me -> refresh (token rotation) -> logout
"""
from __future__ import annotations

import pytest
from jsonschema import validate

from src.schemas import ME_SCHEMA

pytestmark = pytest.mark.auth


@pytest.mark.smoke
def test_login_succeeds_with_valid_credentials(anon_client, settings):
    response = anon_client.login(settings.test_email, settings.test_password)

    assert response.status_code == 200
    body = response.json()
    assert body["user"] == settings.test_email
    assert body["tenant"], "expected a tenant id in the login response"
    # Both the short-lived access cookie and the rotating refresh cookie are set.
    assert "maelstrom_auth" in anon_client.cookies
    assert "maelstrom_refresh" in anon_client.cookies


@pytest.mark.negative
@pytest.mark.parametrize(
    "email, password, case",
    [
        ("test@test.org", "wrong-password", "valid email, wrong password"),
        ("nobody@nowhere.test", "test", "unknown email"),
        ("", "", "empty credentials"),
    ],
)
def test_login_rejects_invalid_credentials(anon_client, email, password, case):
    response = anon_client.login(email, password)
    assert response.status_code in (400, 401), f"expected rejection for: {case}"


@pytest.mark.negative
def test_protected_endpoint_requires_authentication(anon_client):
    # No login -> empty cookie jar -> the API must refuse.
    response = anon_client.get("/products")
    assert response.status_code == 401


def test_me_returns_identity_for_authenticated_user(anon_client, settings):
    anon_client.login_as_test_user()

    response = anon_client.me()
    assert response.status_code == 200
    body = response.json()
    validate(instance=body, schema=ME_SCHEMA)
    assert body["email"] == settings.test_email


def test_refresh_rotates_token_and_keeps_user_authenticated(anon_client):
    anon_client.login_as_test_user()
    original_refresh = anon_client.cookies.get("maelstrom_refresh")

    refreshed = anon_client.refresh()
    assert refreshed.status_code == 200

    # Rotation: a new refresh token replaces the old, and the session still works.
    assert anon_client.cookies.get("maelstrom_refresh") != original_refresh
    assert anon_client.me().status_code == 200


def test_logout_ends_the_session(anon_client):
    anon_client.login_as_test_user()
    assert anon_client.me().status_code == 200

    anon_client.logout()
    assert anon_client.me().status_code == 401
