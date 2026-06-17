"""A thin, intent-revealing wrapper over the MaelstromOps API.

This is the API-testing analogue of a Page Object Model: tests express *intent*
("log in", "list products") and never touch transport details, headers, or URLs.
An httpx ``Client`` keeps a cookie jar, so the auth cookie set at login is reused
automatically on every subsequent request.
"""
from __future__ import annotations

import httpx

from .config import Settings


class ApiClient:
    def __init__(self, settings: Settings, timeout: float = 30.0) -> None:
        self._settings = settings
        self._client = httpx.Client(base_url=settings.base_url, timeout=timeout)

    # -- context-manager sugar so tests can `with ApiClient(...) as c:` ----
    def __enter__(self) -> ApiClient:
        return self

    def __exit__(self, *_exc) -> None:
        self.close()

    def close(self) -> None:
        self._client.close()

    @property
    def cookies(self) -> httpx.Cookies:
        return self._client.cookies

    # -- authentication ----------------------------------------------------
    def login(self, email: str, password: str) -> httpx.Response:
        return self._client.post(
            "/authentication/login", json={"email": email, "password": password}
        )

    def login_as_test_user(self) -> httpx.Response:
        return self.login(self._settings.test_email, self._settings.test_password)

    def logout(self) -> httpx.Response:
        return self._client.post("/authentication/logout")

    def refresh(self) -> httpx.Response:
        return self._client.post("/authentication/refresh")

    def me(self) -> httpx.Response:
        return self._client.get("/authentication/me")

    # -- generic verbs used by resource/CRUD tests -------------------------
    def get(self, path: str, **kwargs) -> httpx.Response:
        return self._client.get(path, **kwargs)

    def post(self, path: str, json=None, **kwargs) -> httpx.Response:
        return self._client.post(path, json=json, **kwargs)

    def put(self, path: str, json=None, **kwargs) -> httpx.Response:
        return self._client.put(path, json=json, **kwargs)

    def delete(self, path: str, **kwargs) -> httpx.Response:
        return self._client.delete(path, **kwargs)
