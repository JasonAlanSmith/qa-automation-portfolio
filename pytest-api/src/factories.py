"""Test-data builders (factories).

Intention-revealing payload builders keep tests readable and resilient: a test
says *what matters* for its scenario and lets the factory supply valid defaults
for everything else. Every name is made unique so tests are independent and can
run in parallel without colliding on data.
"""
from __future__ import annotations

import uuid
from typing import Any


def unique_name(prefix: str = "pytest") -> str:
    """A collision-free name, so concurrently-running tests never clash."""
    return f"{prefix}-{uuid.uuid4().hex[:12]}"


def customer_payload(
    kind_sysid: str,
    theme_sysid: str,
    **overrides: Any,
) -> dict[str, Any]:
    """A valid `CustomerForCreate` body.

    Required by the API: ``name`` + the ``kindSysId``/``themeSysId`` reference FKs
    (resolved at runtime — never hard-coded). Any field can be overridden per test,
    e.g. ``customer_payload(k, t, name="Acme", isIndividual=True)``.
    """
    payload: dict[str, Any] = {
        "name": unique_name("customer"),
        "isIndividual": False,
        "isActive": True,
        "kindSysId": kind_sysid,
        "themeSysId": theme_sysid,
    }
    payload.update(overrides)
    return payload
