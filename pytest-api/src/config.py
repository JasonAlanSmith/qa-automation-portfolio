"""Environment-driven settings.

A first principle of maintainable suites: never hard-code URLs or credentials in
tests. Everything configurable lives here, sourced from environment variables (with
sensible local-dev defaults) and an optional `.env` file.
"""
from __future__ import annotations

import os
from dataclasses import dataclass

from dotenv import load_dotenv

# Load a local .env if present; real environment variables always take precedence.
load_dotenv()


@dataclass(frozen=True)
class Settings:
    base_url: str = os.getenv("MOPS_BASE_URL", "http://localhost:5009/api/v1")
    test_email: str = os.getenv("MOPS_TEST_EMAIL", "test@test.org")
    test_password: str = os.getenv("MOPS_TEST_PASSWORD", "test")


settings = Settings()
